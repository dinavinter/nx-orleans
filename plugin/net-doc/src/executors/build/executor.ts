import { DocfxBuildExecutorSchema } from './schema';
import {DocfxExecutorSchema} from "../docfx/schema";
import {ExecutorContext, logger, ProjectConfiguration, workspaceRoot} from "@nx/devkit";
import {DotNetClient, dotnetFactory} from "@nx-dotnet/dotnet";
import {
  getExecutedProjectConfiguration,
  readInstalledDotnetToolVersion
} from "@nx-dotnet/utils";
import {dirname, resolve} from "path";
 import { existsSync, mkdirSync } from 'fs';
import { execSync } from 'child_process';
import * as fs from "fs";

export const cli = 'docfx';


function normalizeOptions(
  opts: Partial<DocfxExecutorSchema>,
  project: ProjectConfiguration,
  projectName: string,
): DocfxBuildExecutorSchema {
  console.log("normalizeOptions" , opts, project);
  const output = opts.output ?? resolve(workspaceRoot, `generated/${projectName}/_site`);
  const input = opts.input ?? [`${project.root}/**/*.{md,yml}`]

  const build = opts.build?? {
    "content": [
      {
        "files": [
          input
        ]
      }
    ],
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
    output,
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
   const projectDirectory = resolve(workspaceRoot, nxProjectConfiguration.root);
  const options  = normalizeOptions(
    schema,
    nxProjectConfiguration,
    context.projectName as string,
  );

  // dotnetClient.cwd = projectDirectory;
  dotnetClient.cwd = workspaceRoot;


  const outputDirectory = dirname(options.output);
  if (!existsSync(outputDirectory)) {
    mkdirSync(outputDirectory, { recursive: true });
  }

  const config=options.config ?? resolve(outputDirectory, 'docfx.json');
  if (!existsSync(config)) {
    fs.writeFileSync(config, JSON.stringify(options, null, 2));
  }
   console.log("workspace-root: ", workspaceRoot, "\tinput: ", options.input, "\toutput:", options.output, "\tconfig:", config, "\toutputDirectory:", outputDirectory);

  if (!options.skipInstall) {
    ensureCLIToolInstalled(
      context,
      dotnetClient,
      "2.75.3",
    );
  }

  dotnetClient.runTool('docfx', [
    'build',
    config,
    "-o",
    options.output

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
