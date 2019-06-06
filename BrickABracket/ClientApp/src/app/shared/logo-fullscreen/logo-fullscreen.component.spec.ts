import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LogoFullscreenComponent } from './logo-fullscreen.component';

describe('LogoFullscreenComponent', () => {
  let component: LogoFullscreenComponent;
  let fixture: ComponentFixture<LogoFullscreenComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LogoFullscreenComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LogoFullscreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
