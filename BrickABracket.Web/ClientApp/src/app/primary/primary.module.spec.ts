import { PrimaryModule } from './primary.module';

describe('PrimaryModule', () => {
  let primaryModule: PrimaryModule;

  beforeEach(() => {
    primaryModule = new PrimaryModule();
  });

  it('should create an instance', () => {
    expect(primaryModule).toBeTruthy();
  });
});
