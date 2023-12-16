import {   DocfxTOCExecutorSchema} from './schema';
import {ExecutorContext, logger, ProjectConfiguration, workspaceRoot} from "@nx/devkit";
import {DotNetClient, dotnetFactory} from "@nx-dotnet/dotnet";
import {
  getExecutedProjectConfiguration,
  readInstalledDotnetToolVersion
} from "@nx-dotnet/utils";
import {dirname, resolve} from "path";
 import { existsSync, mkdirSync } from 'fs';
import { execSync } from 'child_process';

const cli = 'DocFxTocGenerator';
function normalizeOptions(
  opts: Partial<DocfxTOCExecutorSchema>,
  project: ProjectConfiguration,
  projectName: string,
): DocfxTOCExecutorSchema {
  console.log("normalizeOptions" , opts, project);
  const input = opts.input ?? project.root;
  const output = opts.output ?? input;


  return {
    output, input, ...opts
   };
}


export default async function runExecutor(
  schema: Partial<DocfxTOCExecutorSchema>,
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

 console.log("workspace-root: ", workspaceRoot, "\tinput: ", options.input, "\toutput:", options.output,  "\toutputDirectory:", outputDirectory);

    const cli = ensureCLIToolInstalled(
        context,
        dotnetClient
    );

  const params = args(options);
  console.log("args: ", params);
  dotnetClient.runTool(cli,  params);

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
  dotnetClient: DotNetClient) {
     const cli = 'DocFxTocGenerator';
     const version = '1.17.0';
    const toolVersion =
    readInstalledDotnetToolVersion(cli);

  console.log("toolVersion: ", toolVersion);
  if (toolVersion) {
    if (toolVersion === version) {
      return cli;
    }
    logger.warn(
      `CLI was found, but the version "${toolVersion}" does not match the expected version "${version}" of ${cli} in ${context.projectName}. We reinstalled it such that the version matches, but you may want to review the changes made.`,
    );
  }

  dotnetClient.installTool(cli, version);
  return cli;
}


function args (options: DocfxTOCExecutorSchema): string[]  {
    const args = [];
    if (options.input) {
        args.push('-d', options.input);
    }
    if (options.output) {
        args.push('-o', options.output);
    }
    if (options.verbose) {
        args.push('-v');
    }
    if (options.sequence) {
        args.push('-s');
    }
    if (options.override) {
        args.push('--override');
    }
    if (options.index) {
        args.push('-i');
    }
    if (options.ignore) {
        args.push('-g');
    }
    if (options.multitoc) {
        args.push('-m');
    }
    return args;
}
