"""Build the Deploy enemy source index from a local ANOMALITH installation."""
import argparse
import csv
import hashlib
import re
import shutil
import struct
import subprocess
from collections import defaultdict
from pathlib import Path

import UnityPy
from UnityPy.helpers.TypeTreeGenerator import TypeTreeGenerator
from UnityPy.helpers.TypeTreeNode import TypeTreeNode


SCENE = re.compile(rb"(?P<scene>(?:en|rush|unq|qst)_map_(?P<area>\d{3}[a-z]?)(?:_\d{2})?)\.unity")
BUNDLE = re.compile(rb"\{UnityEngine[^}]*\}[\\/]+StandaloneWindows64[\\/]([a-f0-9]{32}\.bundle)")
ENEMY = re.compile(r"(?:EN_\d+|(?:MID_)?BOSS_\d+)(?=_|$)")
COMPONENTS = {"Event_EnemySpawner", "Event_EnemyRush", "EnemySpawnerSetting"}

# Let the game's stream reader handle the bundle header. Stream all headers
# through one PowerShell process and leave the local bundles untouched.
READ_HEADERS = r"""
$ErrorActionPreference = 'Stop'
$game = [Console]::ReadLine()
$managed = Join-Path $game 'Anomalith_Data/Managed/Unity.Addressables.dll'
$streamType = [Reflection.Assembly]::LoadFrom($managed).GetType('UnityEngine.AddressableAssets.SeekableAesStream')
$output = [Console]::OpenStandardOutput()
while ($name = [Console]::ReadLine()) {
    $file = [IO.File]::OpenRead((Join-Path $game "Anomalith_Data/StreamingAssets/aa/StandaloneWindows64/$name"))
    try {
        $header = [byte[]]::new(128)
        if ($file.Read($header, 0, 128) -ne 128) { throw "Truncated bundle: $name" }
        if ([Text.Encoding]::ASCII.GetString($header, 0, 7) -ne 'UnityFS') {
            $file.Position = 0
            $salt = [Text.Encoding]::UTF8.GetBytes([IO.Path]::GetFileNameWithoutExtension($name))
            $reader = [Activator]::CreateInstance($streamType, @($file, 'AtariNatsume', $salt))
            try {
                if ($reader.Read($header, 0, 128) -ne 128) { throw "Truncated header: $name" }
            } finally { $reader.Dispose() }
        }
        $output.Write($header, 0, 128)
    } finally { $file.Dispose() }
}
"""


def number(data, offset):
    if offset < 0 or offset + 4 > len(data):
        return 0
    return struct.unpack_from("<I", data, offset)[0]


def catalog_scenes(catalog, areas):
    scenes = {}
    for match in SCENE.finditer(catalog):
        area = "map_" + match["area"].decode("ascii")
        if area not in areas:
            continue
        # The scene location points back to the path string. Its dependency
        # array starts with the bundle that owns the scene.
        start = match.end()
        candidates = set()
        for pointer in re.finditer(re.escape(struct.pack("<I", match.start())), catalog[start:start + 240]):
            location = start + pointer.start()
            dependency = number(catalog, location + 20)
            bundle = number(catalog, dependency)
            internal_path = number(catalog, bundle + 4)
            path = BUNDLE.match(catalog, internal_path)
            if path:
                candidates.add(path[1].decode("ascii"))
        if len(candidates) != 1:
            raise ValueError(f"Expected one bundle for {match[0]!r}, found {candidates}")
        scenes[match["scene"].decode("ascii")] = (area, candidates.pop())
    return scenes


def read_rows(csv_dir, name):
    with (csv_dir / f"{name}.csv").open(encoding="utf-8-sig", newline="") as source:
        return list(csv.reader(source))


def loot_tables(csv_dir):
    rows = read_rows(csv_dir, "EnemyTableData")
    fields = {row[6]: row for row in rows[3:] if len(row) > 6}
    tables = {}
    for column, name in enumerate(rows[2]):
        if not name.endswith("_TABLE"):
            continue
        def cell(field):
            row = fields[field]
            return row[column] if column < len(row) else ""
        items = set()
        for row in rows[3:]:
            if len(row) <= column + 1 or len(row) <= 6 or not row[6].startswith("ENEMY_DROP_"):
                continue
            item, weight = row[column:column + 2]
            if re.fullmatch(r"ITM_[A-Z0-9_]+", item) and re.fullmatch(r"-?\d+", weight) and int(weight):
                items.add(item)
        tables[name[:-6]] = (items, cell("TABLE").split("/"), cell("MAP_ID").split("/"))
    return tables


def native_table(enemy, variant, tables):
    family = ENEMY.match(enemy)
    if not family:
        return None
    family = family[0]
    if not 0 <= variant <= 11:
        return None
    suffix = re.search(r"_Var[A-I](?=_|$)", enemy)
    if suffix:
        variant = ord(suffix[0][-1]) - ord("A")
    variant_id = f"{family}_Var{chr(ord('A') + variant)}" if variant <= 8 else family
    return next((key for key in (enemy if suffix else variant_id, variant_id, family, family + "_VarA")
                 if key in tables), None)


def allowed_map(filters, area):
    # The CSV's _SP entries are for special dungeons, not normal Deploy.
    return filters == [""] or area in filters or area[:5] in filters


def matching_drops(enemy, variant, area, tables):
    normal = native_table(enemy, variant, tables)
    if not normal:
        return
    family = ENEMY.match(enemy)[0]
    special = next((key for key in (normal + "_SP", family + "_SP", family + "_VarA_SP") if key in tables), None)
    for key in (normal, special):
        if key is None:
            continue
        _, inherited, _ = tables[key]
        for table in (key, *inherited):
            if table not in tables:
                continue
            items, _, filters = tables[table]
            if allowed_map(filters, area):
                for item in items:
                    yield item, normal


def document_keys(csv_dir, tables):
    keys = {}
    for row in read_rows(csv_dir, "Document"):
        if len(row) <= 7 or row[7] != "ENEMY":
            continue
        name = row[6]
        family = ENEMY.match(name)
        if family:
            suffix = re.search(r"_Var[A-I]$", name)
            table = family[0] + (suffix[0] if suffix else "")
            if table in tables:
                keys[table] = name
    return keys


def random_variant_exclusions(game):
    data_dir = game / "Anomalith_Data"
    env = UnityPy.load(str(data_dir / "level1"), str(data_dir / "globalgamemanagers.assets"))
    scene = next(file for file in env.files.values() if file.name == "level1")
    generator = TypeTreeGenerator(scene.unity_version)
    generator.load_local_game(str(game))
    env.typetree_generator = generator
    for obj in scene.objects.values():
        if obj.type.name != "MonoBehaviour":
            continue
        try:
            script = obj.parse_monobehaviour_head().m_Script.deref_parse_as_object()
        except (FileNotFoundError, KeyError, ValueError):
            continue
        if script.m_ClassName != "DataBase":
            continue
        # The generated tree represents lists of strings as strings. Unity
        # serializes them as vectors, and aligns the MonoBehaviour header.
        nodes = [
            TypeTreeNode(node.m_Level,
                         "vector" if node.m_Name in ("typeList", "ignoreNinshikiObjList") else node.m_Type,
                         node.m_Name, 0, 0,
                         m_MetaFlag=0x4000 if node.m_Name == "m_Enabled" else node.m_MetaFlag)
            for node in generator.get_nodes("Assembly-CSharp.dll", "DataBase")
        ]
        data = obj.read_typetree(nodes=TypeTreeNode.from_list(nodes))
        return {row["enemyID"]: set(row["ignoreVariant"]) for row in data["ignoreVariantList"]}
    raise ValueError("Could not find the game's random variant exclusions")


def available_variants(catalog):
    variants = defaultdict(set)
    for match in re.finditer(rb"((?:EN_\d+|(?:MID_)?BOSS_\d+))_[A-Z0-9_]+_Var([A-I])\.prefab", catalog):
        variants[match[1].decode("ascii")].add(match[2][0] - ord("A"))
    return variants


def resolve_variants(enemy, variant, ignored, available, exclusions):
    if variant != 12:
        yield variant
        return
    family = ENEMY.match(enemy)
    if family:
        key = family[0]
        yield from available[key] - exclusions.get(key, set()) - set(ignored)


def spawn_entries(component, data):
    if component == "Event_EnemySpawner":
        if not data["isSpecialDungeonAdd"]:
            yield data["EnemyID"], data["Variant"], data["ignoreVariant"]
    elif component == "EnemySpawnerSetting":
        for entry in data["enemyInfo"]:
            yield entry["enemyID"], entry["Variant"], []
    elif component == "Event_EnemyRush":
        for table in data["IDTableList"]:
            if table["isSpecialDungeonAdd"]:
                continue
            for entry in table["enemyInfoList"]:
                if entry["count"] <= 0:
                    continue
                for candidate in table["overrideEnemyInfo"] or [entry]:
                    yield candidate["enemyID"], candidate["Variant"], entry["ignoreVariant"]


def scene_spawns(game, aa, catalog, scenes, destinations):
    scripts = re.search(rb"([a-f0-9]{32}\.bundle).{16}\x0b\x00\x00\x00monoscripts", catalog, re.DOTALL)
    if scripts is None:
        raise ValueError("Could not find the shared script metadata bundle")
    script_bundle = scripts[1].decode("ascii")
    names = sorted({bundle for _, bundle in scenes.values()} | {script_bundle})
    shell = shutil.which("pwsh") or shutil.which("powershell")
    headers = subprocess.check_output(
        [shell, "-NoProfile", "-Command", READ_HEADERS],
        input=("\n".join([str(game), *names]) + "\n").encode(),
    )
    if len(headers) != 128 * len(names):
        raise ValueError("Incomplete encounter bundle headers")
    header_by_name = {name: headers[i * 128:(i + 1) * 128] for i, name in enumerate(names)}

    def bundle_data(name):
        with (aa / "StandaloneWindows64" / name).open("rb") as source:
            source.seek(128)
            return header_by_name[name] + source.read()

    script_data = bundle_data(script_bundle)
    generator = None
    for key, (area, name) in sorted(scenes.items()):
        mapped_areas = destinations.get((key.split("_", 1)[0], area), {area})
        env = UnityPy.Environment()
        env.load_file(script_data, name=script_bundle)
        bundle = env.load_file(bundle_data(name), name=name)
        if not hasattr(bundle, "files"):
            raise ValueError(f"Could not open encounter scene {key}")
        for scene in bundle.files.values():
            if not hasattr(scene, "objects"):
                continue
            if generator is None:
                generator = TypeTreeGenerator(scene.unity_version)
                generator.load_local_game(str(game))
            env.typetree_generator = generator
            for obj in scene.objects.values():
                if obj.type.name != "MonoBehaviour":
                    continue
                try:
                    head = obj.parse_monobehaviour_head()
                    script = head.m_Script.deref_parse_as_object()
                except (FileNotFoundError, KeyError, ValueError):
                    continue
                component = script.m_ClassName
                if component in COMPONENTS:
                    try:
                        data = obj.parse_as_dict()
                        for enemy, variant, ignored in spawn_entries(component, data):
                            for destination in mapped_areas:
                                yield destination, enemy, variant, tuple(ignored)
                    except (ValueError, KeyError, TypeError, EOFError) as error:
                        raise ValueError(f"Could not read {component} in {key}") from error


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("game", type=Path)
    parser.add_argument("--output", type=Path, default=Path(__file__).resolve().parents[1] / "Data/EnemySources.tsv")
    args = parser.parse_args()
    game = args.game.resolve()
    streaming = game / "Anomalith_Data/StreamingAssets"
    aa = streaming / "aa"
    csv_dir = streaming / "CSV"
    catalog = (aa / "catalog.bin").read_bytes()
    areas = {row[6] for row in read_rows(csv_dir, "TableData")
             if len(row) > 6 and re.fullmatch(r"map_[0-5]\d{2}[a-z]?", row[6])}
    scenes = catalog_scenes(catalog, areas)
    tables = loot_tables(csv_dir)
    document = document_keys(csv_dir, tables)
    aliases = {row[6]: row for row in read_rows(csv_dir, "ConvertMapName") if len(row) > 6 and row[6] in areas}
    destinations = defaultdict(set)
    for area in areas:
        row = aliases.get(area, [])
        for kind, column in (("en", 9), ("unq", 10)):
            scene_area = (row[column] if len(row) > column else "") or area
            destinations[kind, scene_area].add(area)
    exclusions = random_variant_exclusions(game)
    available = available_variants(catalog)
    encounters = {(area, enemy, variant)
                  for area, enemy, configured, ignored in scene_spawns(game, aa, catalog, scenes, destinations)
                  for variant in resolve_variants(enemy, configured, ignored, available, exclusions)}
    entries = {(item, area, document.get(enemy_table, ""))
               for area, enemy, variant in encounters
               for item, enemy_table in matching_drops(enemy, variant, area, tables)}
    for item, area in (("ITM_ENE_023", "map_209"), ("ITM_ENE_025", "map_105")):
        if not any(entry[:2] == (item, area) for entry in entries):
            raise ValueError(f"Known encounter missing from extracted data: {item} in {area}")
    args.output.parent.mkdir(parents=True, exist_ok=True)
    with args.output.open("w", encoding="utf-8", newline="\n") as output:
        output.write(f"# catalog SHA256 {hashlib.sha256(catalog).hexdigest()}\n")
        for entry in sorted(entries):
            output.write("\t".join(entry) + "\n")
    print(f"Indexed {len(scenes)} scenes, {len(encounters)} area/enemy/variant matches, "
          f"{len({r[0] for r in entries})} materials, {len({r[1] for r in entries})} areas, {len(entries)} source rows.")


if __name__ == "__main__":
    main()
