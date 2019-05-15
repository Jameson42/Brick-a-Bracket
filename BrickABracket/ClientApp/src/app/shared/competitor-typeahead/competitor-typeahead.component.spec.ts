import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CompetitorTypeaheadComponent } from './competitor-typeahead.component';

describe('CompetitorTypeaheadComponent', () => {
  let component: CompetitorTypeaheadComponent;
  let fixture: ComponentFixture<CompetitorTypeaheadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CompetitorTypeaheadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompetitorTypeaheadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
