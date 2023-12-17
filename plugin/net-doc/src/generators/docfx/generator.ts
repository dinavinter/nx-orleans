import {
  addProjectConfiguration,
  formatFiles,
  generateFiles, getWorkspaceLayout, joinPathFragments, ProjectConfiguration, readProjectConfiguration,
  Tree, updateProjectConfiguration, workspaceRoot,
} from '@nx/devkit';
import * as path from 'path';
import { DocfxGeneratorSchema } from './schema';
import {findProject} from "@nx/eslint-plugin/src/utils/runtime-lint-utils";
import {findProjectFileInPath} from "@nx-dotnet/utils";

export async function docfxGenerator(
  tree: Tree,
  options: DocfxGeneratorSchema
) {
  const docsProjectName = options.docsProject ?? `${options.name ?? "docs"}`;
  const docsPath = `generated/${options.name}`;
  const project = readProjectConfiguration(tree, options.project);
  const docsRoot = docsPath;
  //docsProjectRoot(tree,docsPath,);
  const reference = `${docsPath}/references/${options.project}`;
  const site = `${docsRoot}/site`;
  project.targets ??= {};
  project.targets.doc = {
    executor: '@nx-net/docs:docfx',
    options: {
      output: reference,
      input: project.root,
      outputProject: `${options.name}`

    }
  };
  let srcs =  [project.root];

  async function addOrUpdateProject() {
    try {
      const docsProject = readProjectConfiguration(tree, docsProjectName);
      console.log('found', `${workspaceRoot}\{docsRoot}`);

      docsProject.targets ??= {};
      docsProject.targets.build ??= {
        executor: 'nx:noop',
        outputs: [docsPath],
      };
      docsProject.targets.doc ??= {
        executor: '@nx-net/docs:docfx',
        options: {
          output: reference,
          inputs: [] as string[],
          config: `${docsPath}/docfx.json`
        }
      };

      srcs.push(...docsProject.targets.doc.options.inputs);

      docsProject.targets.doc.inputs = srcs;

      updateProjectConfiguration(tree, docsProjectName, docsProject);
    }catch (e) {

      addProjectConfiguration(tree, options.name, {
        name: docsProjectName,
        root: docsPath,
        projectType: 'library',
        targets: {
          build: {
            executor: 'nx:noop',
            outputs: [docsPath],
          },
          "doc": {
            "executor": "@nx-net/docs:build",
            options: {
              config: `docfx.json`,
              inputs: [project.root],
            }
          }
        },
      });
    }
  }

  await addOrUpdateProject();
  updateProjectConfiguration(tree, options.project, project);

  console.log('projectRoot', docsPath);
  console.log('subt', {
    reference: reference,
    output: site,
    src: project.root,
    ...options,

  });
  generateFiles(tree, path.join(__dirname, 'files'), docsPath, {
    references: reference,
    output: site,
    src: srcs,
    ...options,

  });
  await formatFiles(tree);
}

function generateShellProject(
  host: Tree,
  options: DocfxGeneratorSchema & { docsProject: string },
) {
  const root = docsProjectRoot(host, options.docsProject);
  const targets: ProjectConfiguration['targets'] = {};
     // If typescript lib is buildable,
    // then this lib must be too. It seems
    // a little silly, but we **need** this target.
    targets.build = {
      executor: 'nx:noop',
      outputs: [root],
    };

      targets.codegen = {
        command: 'docfx build --output ./dist',
        options: {
          openapiJsonPath: `${docsProjectRoot(
            host,
            options.docsProject,
          )}/swagger.json`,
          outputProject: `generated-${options.name}`,
        },
        dependsOn: ['^swagger'],
      };


  addProjectConfiguration(host, options.docsProject, {
    root,
    targets,
    implicitDependencies: [options.project],
  });
}

function docsProjectRoot(host: Tree, swaggerProject: string) {
  return joinPathFragments(
    'generated',
    swaggerProject,
  );
}

export default docfxGenerator;
