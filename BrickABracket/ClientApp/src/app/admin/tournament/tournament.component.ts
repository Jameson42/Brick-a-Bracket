import { Component, OnInit } from '@angular/core';
import { Tournament } from '../../core/tournaments/tournament';
import { Observable } from 'rxjs';
import { TournamentService } from '../../core/tournaments/tournament.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-tournament',
  templateUrl: './tournament.component.html',
  styleUrls: ['./tournament.component.css']
})
export class TournamentComponent implements OnInit {

  private tournament$: Observable<Tournament>;
  private isNew: boolean;

  constructor(
    private tournaments: TournamentService,
    private route: ActivatedRoute,
    private router: Router
    ) { }

  ngOnInit() {
    this.route.url.subscribe(e => {
      const path = e[e.length - 1].path;
      this.isNew = path === 'new';
      if (this.isNew) {
        this.tournament$ = Observable.create(observer => {
          observer.next(new Tournament());
          observer.complete();
        });
      } else {
        if (path !== 'tournament') {
          this.tournaments.setActive(Number(path));
        }
        this.tournament$ = this.tournaments.tournament;
      }
    });
  }

  save(tournament: Tournament) {
    if (this.isNew) {
      this.tournaments.create(tournament.name, tournament.tournamentType)
        .then(_ => {
          this.router.navigate(['/admin/tournament']);
      });
    } else {
      this.tournaments.update(tournament);
    }
    console.log(tournament);
  }
}
