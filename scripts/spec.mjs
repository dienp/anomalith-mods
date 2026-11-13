import { spawnSync } from 'node:child_process';
import { realpathSync } from 'node:fs';
import { dirname, relative, resolve, sep } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = realpathSync(resolve(dirname(fileURLToPath(import.meta.url)), '..'));
const [target, ...args] = process.argv.slice(2);

function fail(message) {
  console.error(message);
  process.exit(1);
}

if (!target || target === '--help' || target === '-h') {
  console.log('Usage: npm run spec -- <apps/name|packages/name> <openspec arguments>');
  process.exit(target ? 0 : 1);
}

const parts = target.split('/');
if (parts.length !== 2 || !['apps', 'packages'].includes(parts[0]) ||
    !/^[a-z0-9]+(?:-[a-z0-9]+)*$/.test(parts[1])) {
  fail('Target must be apps/<name> or packages/<name>, using lowercase kebab-case.');
}

let cwd;
try {
  cwd = realpathSync(resolve(root, target));
} catch {
  fail(`Create ${target}/ before running OpenSpec there.`);
}
if (relative(root, cwd) !== parts.join(sep)) {
  fail('Target must resolve to its own directory inside this repository.');
}

const cli = resolve(root, 'node_modules/@fission-ai/openspec/bin/openspec.js');
const result = spawnSync(process.execPath, [cli, ...args], {
  cwd,
  stdio: 'inherit',
});
if (result.error) fail(result.error.message);
if (result.signal) fail(`OpenSpec terminated by ${result.signal}.`);
process.exit(result.status ?? 1);
