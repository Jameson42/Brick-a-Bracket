import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StandingsTableComponent } from './standings-table.component';

describe('StandingsTableComponent', () => {
  let component: StandingsTableComponent;
  let fixture: ComponentFixture<StandingsTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StandingsTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StandingsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
