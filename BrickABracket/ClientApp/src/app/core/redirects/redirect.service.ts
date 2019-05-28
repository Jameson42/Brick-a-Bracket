import { Injectable, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, distinctUntilChanged } from 'rxjs/operators';

import { TournamentService } from '@bab/core/tournaments/tournament.service';

@Injectable({
  providedIn: 'root'
})
export class RedirectService implements OnDestroy {

  private destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tournaments: TournamentService
  ) {
    this.tournaments.metadata.pipe(
      takeUntil(this.destroy$),
      distinctUntilChanged((x, y) => x.matchIndex === y.matchIndex &&
        x.roundIndex === y.roundIndex && x.categoryIndex === y.categoryIndex),
    ).subscribe(data => {
      if (data.matchIndex > -1) {
        this.navigate('match');
      } else if (data.roundIndex > -1) {
        this.navigate('round');
      } else if (data.categoryIndex > -1) {
        this.navigate('category');
      } else {
        this.navigate('tournament');
      }
    });
  }

  navigate(path: string) {
    this.router.navigate(['/' + this.route.outlet + '/' + path], { skipLocationChange: true });
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

}
