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
