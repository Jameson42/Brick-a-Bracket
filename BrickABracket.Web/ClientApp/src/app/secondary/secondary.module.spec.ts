import { SecondaryModule } from './secondary.module';

describe('SecondaryModule', () => {
  let secondaryModule: SecondaryModule;

  beforeEach(() => {
    secondaryModule = new SecondaryModule();
  });

  it('should create an instance', () => {
    expect(secondaryModule).toBeTruthy();
  });
});
