import { DocfxExecutorSchema } from './schema';
import executor from './executor';

const options: DocfxExecutorSchema = {};

describe('Docfx Executor', () => {
  it('can run', async () => {
    const output = await executor(options);
    expect(output.success).toBe(true);
  });
});
