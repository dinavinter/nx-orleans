import {
  ExecutorContext,
  logger,
  ProjectConfiguration,
  workspaceRoot,
} from '@nx/devkit';

import {dirname, relative, resolve} from 'path';

import { DotNetClient, dotnetFactory } from '@nx-dotnet/dotnet';
import {
  getExecutedProjectConfiguration,
  getProjectFileForNxProject,
  iterateChildrenByPath,
  readConfig,
  readXml,
  readInstalledDotnetToolVersion,
} from '@nx-dotnet/utils';


import { DocfxExecutorSchema } from './schema';
import { existsSync, mkdirSync } from 'fs';
import { execSync } from 'child_process';
import * as fs from "fs";
import {updateFile} from "@nx/plugin/testing";

export const cli = 'docfx';

function normalizeOptions(
  opts: Partial<DocfxExecutorSchema>,
  project: ProjectConfiguration,
  csProjFilePath: string,
  projectName: string,
): DocfxExecutorSchema {
  console.log("normalizeOptions" , opts, project);
  const output = opts.output ?? resolve(workspaceRoot, `generated/docs/${projectName}/specs`);
  const input = resolve(workspaceRoot,opts.input) ?? resolve(workspaceRoot,`${project.root}`);

  const metadata = opts.metadata?? [{
     src: [{
       "files": ["**.csproj"],
       "exclude": ["**/bin/**", "**/obj/**", "**/[Tt]ests/**"],
       src: resolve(workspaceRoot, input)
       // src: input

     }],
    "includePrivateMembers": false,
    "disableGitFeatures": false,
    "disableDefaultFilter": false,
    "noRestore": false,
    "namespaceLayout": "nested",
    "enumSortOrder": "declaringOrder"
  }];
const build = {
  "content": [
    {
      "files": [
        "**/*.{md,yml}",
       ]
    }
  ],
  "dest": "_site",
  "globalMetadata": {
    "_appTitle": "QuickStart Documentation",
    "_appName": "QUickStart Documentation",
    "_appLogoPath": "images/logo.png",
    "_appFaviconPath": "images/favicon.ico",
    "_enableSearch": true,
    "_enableNewTab": true
  }
};

  return {
     metadata: metadata,
    output:output,
    build:build,
     ...opts
  };
}

export default async function runExecutor(
  schema: Partial<DocfxExecutorSchema>,
  context: ExecutorContext,
  dotnetClient: DotNetClient = new DotNetClient(dotnetFactory(), workspaceRoot),
) {
  const nxProjectConfiguration = getExecutedProjectConfiguration(context);
  const csProjFilePath = await getProjectFileForNxProject(
    nxProjectConfiguration,
  );
  const projectDirectory = resolve(workspaceRoot, nxProjectConfiguration.root);
  const options = normalizeOptions(
    schema,
    nxProjectConfiguration,
    csProjFilePath,
    context.projectName as string,
  );

  // dotnetClient.cwd = options.output;


  // const outputDirectory = dirname(options.output);
  const outputDirectory = resolve(workspaceRoot,options.output);
  if (!existsSync(outputDirectory)) {
    mkdirSync(outputDirectory, { recursive: true });
  }

  const config=options.config ?? resolve(outputDirectory, 'docfx.json');
  fs.writeFileSync(config, JSON.stringify(options, null, 2));
    if (!options.skipInstall) {
    ensureCLIToolInstalled(
      context,
      dotnetClient,
      "2.73.2",
    );
  }

  dotnetClient.runTool('docfx', [
    'metadata',
    config,
    "-o",
    options.output,
    "--outputFormat",
    options.format ?? "Markdown"

   ]);

  try {
    const isInstalled = require.resolve('prettier');
    if (isInstalled) {
      execSync(`npx -y prettier --write ${options.output}`);
    }
  } catch {
    // Its not a huge deal if prettier isn't installed or fails...
    // We'll just leave the file as is and let the user decide what to do.
  }

  return {
    success: true,
  };
}

function ensureCLIToolInstalled(
  context: ExecutorContext,
  dotnetClient: DotNetClient,
  version: string,
) {
  const installedSwaggerVersion =
    readInstalledDotnetToolVersion(cli);

  if (installedSwaggerVersion) {
    if (installedSwaggerVersion === version) {
      return;
    }
    logger.warn(
      `Swagger CLI was found, but the version "${installedSwaggerVersion}" does not match the expected version "${version}" of Swashbuckle.AspNetCore in ${context.projectName}. We reinstalled it such that the version matches, but you may want to review the changes made.`,
    );
  }

  dotnetClient.installTool(cli, version);
}
