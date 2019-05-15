import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassificationSelectComponent } from './classification-select.component';

describe('ClassificationSelectComponent', () => {
  let component: ClassificationSelectComponent;
  let fixture: ComponentFixture<ClassificationSelectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClassificationSelectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClassificationSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
