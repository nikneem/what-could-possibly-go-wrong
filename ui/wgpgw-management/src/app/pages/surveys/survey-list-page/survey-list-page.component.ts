import { Component, OnDestroy, OnInit } from '@angular/core';
import { SurveysService } from '../../../services/surveys.service';
import { ISurvey } from '../../../models/survey.models';
import { catchError, map, of, Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { SurveyCreateDialogComponent } from '../components/survey-create-dialog/survey-create-dialog.component';

@Component({
  selector: 'wgpgw-survey-list-page',
  standalone: false,
  templateUrl: './survey-list-page.component.html',
  styleUrl: './survey-list-page.component.scss',
})
export class SurveyListPageComponent implements OnInit, OnDestroy {
  surveys: Array<ISurvey> = [];
  isLoading = false;
  displayedColumns: string[] = ['code', 'name', 'expiresOn'];
  private surveysSubscription?: Subscription;
  constructor(
    private surveysService: SurveysService,
    private dialog: MatDialog
  ) {}

  createSurvey(): void {
    let dialogRef = this.dialog.open(SurveyCreateDialogComponent, {
      width: '600px',
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadSurveys();
      }
    });
  }

  private loadSurveys(): void {
    this.surveysSubscription?.unsubscribe();
    if (!this.isLoading) {
      this.isLoading = true;
      this.surveysSubscription = this.surveysService
        .getSurveys()
        .pipe(
          map((response) => response.data),
          catchError((error) => {
            console.error(error);
            return of([]);
          })
        )
        .subscribe((surveys: Array<ISurvey>) => {
          this.isLoading = false;
          this.surveys = surveys;
        });
    }
  }

  ngOnInit(): void {
    this.loadSurveys();
  }

  ngOnDestroy(): void {
    this.surveysSubscription?.unsubscribe();
  }
}
