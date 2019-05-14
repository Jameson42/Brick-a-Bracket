import { Component, OnInit } from '@angular/core';
import { Tournament } from '../../core/tournaments/tournament';
import { Observable, Observer } from 'rxjs';
import { TournamentService } from '../../core/tournaments/tournament.service';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-tournament',
  templateUrl: './tournament.component.html',
  styleUrls: ['./tournament.component.css']
})
export class TournamentComponent implements OnInit {

  private tournament$: Observable<Tournament>;
  private isNew: boolean;
  private id: number;

  constructor(
    private tournaments: TournamentService,
    private route: ActivatedRoute,
    private router: Router
    ) { }

  ngOnInit() {
    this.tournament$ = this.route.paramMap.pipe(
      tap(params => {
        this.id = Number(params.get('id'));
        this.isNew = this.id <= 0;
      }),
      switchMap(_ => {
        if (this.isNew) {
          return new Observable<Tournament>(
            (observer: Observer<Tournament>) => {
            observer.next(new Tournament());
            observer.complete();
          });
        }
        this.tournaments.setActive(this.id);
        return this.tournaments.tournament;
      })
    );
  }

  save(tournament: Tournament) {
    if (this.isNew) {
      this.tournaments.create(tournament.name, tournament.tournamentType)
        .then(result => {
          this.router.navigate(['/admin/tournament', { id: result }]);
      });
    } else {
      this.tournaments.update(tournament);
    }
    console.log(tournament);
  }
}
