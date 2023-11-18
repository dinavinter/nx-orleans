import { DocfxBuildExecutorSchema } from './schema';
import executor from './executor';

const options: DocfxBuildExecutorSchema = {};

describe('DocfxBuild Executor', () => {
  it('can run', async () => {
    const output = await executor(options);
    expect(output.success).toBe(true);
  });
});
