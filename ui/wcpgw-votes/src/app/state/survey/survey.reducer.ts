import { createReducer, on } from '@ngrx/store';
import { initialSurveyState } from './survey.state';
import { SurveyActions } from './survey.actions';

export const surveysReducer = createReducer(
  initialSurveyState,
  on(SurveyActions.surveyLoad, (state) => ({
    ...state,
    isLoading: true,
    errorMessage: undefined,
  })),
  on(SurveyActions.surveyLoaded, (state, { survey }) => ({
    ...state,
    survey,
    isLoading: false,
  })),
  on(SurveyActions.surveyLoadError, (state, { errorMessage }) => ({
    ...state,
    errorMessage,
    isLoading: false,
  })),

  on(SurveyActions.questionActivated, (state, { question }) => ({
    ...state,
    activeQuestion: question,
  }))
);
