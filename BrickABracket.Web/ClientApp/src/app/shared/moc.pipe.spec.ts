import { MocPipe } from './moc.pipe';

describe('MocPipe', () => {
  it('create an instance', () => {
    const pipe = new MocPipe();
    expect(pipe).toBeTruthy();
  });
});
