import { Injectable, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, distinctUntilChanged } from 'rxjs/operators';

import { TournamentService } from '@bab/core/tournaments/tournament.service';

@Injectable({
  providedIn: 'root'
})
export class RedirectService implements OnDestroy {

  private destroy$: Subject<boolean>;

  constructor(
    private router: Router,
    private tournaments: TournamentService
  ) {
  }

  start(prefix: string) {
    if (this.destroy$ != null && !this.destroy$.closed) {
      return;
    }
    this.cancel();
    this.destroy$ = new Subject<boolean>();
    this.tournaments.metadata.pipe(
      takeUntil(this.destroy$),
      distinctUntilChanged((x, y) => x.matchIndex === y.matchIndex &&
        x.roundIndex === y.roundIndex && x.categoryIndex === y.categoryIndex),
    ).subscribe(data => {
      if (data.matchIndex > -1) {
        this.navigate('match', prefix);
      } else if (data.roundIndex > -1) {
        this.navigate('round', prefix);
      } else if (data.categoryIndex > -1) {
        this.navigate('category', prefix);
      } else {
        this.navigate('tournament', prefix);
      }
    });
  }

  navigate(path: string, prefix: string) {
    this.router.navigate(['/' + prefix + '/' + path]);
  }

  cancel() {
    if (this.destroy$ == null || this.destroy$.closed) {
      return;
    }
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  ngOnDestroy(): void {
    this.cancel();
  }

}
