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
  projectName: string,
): DocfxExecutorSchema {
  console.log("normalizeOptions" , opts, project);
  // console.log("project.root", opts.outputProject);
  const dest = opts.dest ?? `specs/${projectName}`;
  const output = opts.output ?? resolve(workspaceRoot, `${projectName}/docs`, dest);
  console.log("dest", dest, "output", output);

  const input = resolve(workspaceRoot,opts.input) ?? resolve(workspaceRoot,`${project.root}`);
   const metadata = opts.metadata?? [{
     src: [{
       "files": ["**.csproj"],
       "exclude": ["**/bin/**", "**/obj/**", "**/[Tt]ests/**"],
        src: resolve(workspaceRoot, input)
       // src: input

     }],
     // "dest": dest,
    "includePrivateMembers": false,
    "disableGitFeatures": false,
    "disableDefaultFilter": false,
    "noRestore": false,
    "namespaceLayout": "nested",
    "enumSortOrder": "declaringOrder"
   }];

  return {
     metadata: metadata,
    output:output,
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
    csProjFilePath
  );

  // dotnetClient.cwd = options.output;


  // const outputDirectory = dirname(options.output);
  const outputDirectory = resolve(workspaceRoot,options.output);
  if (!existsSync(outputDirectory)) {
    mkdirSync(outputDirectory, { recursive: true });
  }

  const config=options.config ?? resolve(outputDirectory, 'temp-docfx.json');
  console.log("input: ", options.input, "\toutput:", options.output, "\tconfig:", config, "\toutputDirectory:", outputDirectory);

  fs.writeFileSync(config, JSON.stringify(options, null, 2));
    if (!options.skipInstall) {
    ensureCLIToolInstalled(
      context,
      dotnetClient,
      "2.75.3",
    );
  }

  dotnetClient.runTool('docfx', [
    'metadata',
    config,
    "-o",
    options.output,
    "--outputFormat",
    options.format ?? "markdown",
    "--globalNamespaceId",
    options.globalNamespaceId ?? "",


   ]);

    if(existsSync(resolve(outputDirectory, 'temp-docfx.json'))) {
      fs.unlinkSync(resolve(outputDirectory, 'temp-docfx.json'));
    }
    fs.writeFileSync(resolve(outputDirectory, 'metadata.json'), JSON.stringify(options.metadata, null, 2));

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
