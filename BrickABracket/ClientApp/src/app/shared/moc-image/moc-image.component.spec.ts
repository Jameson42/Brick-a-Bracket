import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MocImageComponent } from './moc-image.component';

describe('MocImageComponent', () => {
  let component: MocImageComponent;
  let fixture: ComponentFixture<MocImageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MocImageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MocImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
