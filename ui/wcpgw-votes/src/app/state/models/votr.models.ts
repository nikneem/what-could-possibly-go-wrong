export interface IResponseModel<TItem> {
  isSuccess: boolean;
  errorMessage?: string;
  data: TItem;
}

export interface IRealtimeConnectionResponse {
  webPubsubEndpointUrl: string;
}

export interface IVoteCreateRequest {
  surveyId: string;
  questionId: string;
  answerId: string;
}

export interface IQuestionVotes {
  surveyId: string;
  surveyCode: string;
  questionId: string;
  question: string;
  answers: Array<IQuestionVoteAnswers>;
}

export interface IQuestionVoteAnswers {
  answerId: string;
  name: string;
  votes: number;
  percentage: number;
}
