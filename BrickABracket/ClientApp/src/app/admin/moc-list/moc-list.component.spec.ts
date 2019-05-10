import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MocListComponent } from './moc-list.component';

describe('MocListComponent', () => {
  let component: MocListComponent;
  let fixture: ComponentFixture<MocListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MocListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MocListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
