import { writeFileSync, existsSync, mkdirSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';
import { config } from 'dotenv';

const __dirname = dirname(fileURLToPath(import.meta.url));
const rootEnvPath = join(__dirname, '..', '..', '.env');
const production = process.argv[2] === 'production';

if (existsSync(rootEnvPath)) {
  config({ path: rootEnvPath, quiet: true });
}

const apiUrl = process.env.ANGULAR_API_URL ?? 'http://localhost:5283';
const appTitle =
  process.env.ANGULAR_APP_TITLE ?? process.env.API_TITLE ?? 'Invoice Generator';

const outDir = join(__dirname, '..', 'src', 'environments');
mkdirSync(outDir, { recursive: true });

const content = `// Gerado por scripts/sync-env-from-root.mjs a partir do .env na raiz do repositório.
export const environment = {
  production: ${production},
  apiUrl: ${JSON.stringify(apiUrl)},
  appTitle: ${JSON.stringify(appTitle)},
} as const;
`;

const outFile = join(outDir, 'environment.ts');
writeFileSync(outFile, content, 'utf8');
