import { createActionGroup, props } from '@ngrx/store';
import { IQuestion, ISurvey } from './survey.models';

export const SurveyActions = createActionGroup({
  source: 'Surveys',
  events: {
    surveyLoad: props<{ surveyCode: string }>(),
    surveyLoaded: props<{ survey: ISurvey }>(),
    surveyLoadError: props<{ errorMessage?: string }>(),

    questionActivated: props<{ question: IQuestion }>(),
  },
});
