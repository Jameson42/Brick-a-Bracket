import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CompetitorListComponent } from './competitor-list.component';

describe('CompetitorListComponent', () => {
  let component: CompetitorListComponent;
  let fixture: ComponentFixture<CompetitorListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CompetitorListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompetitorListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
