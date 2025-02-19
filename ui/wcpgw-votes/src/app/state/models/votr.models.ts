export interface IResponseModel<TItem> {
  isSuccess: boolean;
  errorMessage?: string;
  data: TItem;
}

export interface IRealtimeConnectionResponse {
  webPubsubEndpointUrl: string;
}
