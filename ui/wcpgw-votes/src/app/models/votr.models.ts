export interface IResponseModel<TItem> {
  isSuccess: boolean;
  errorMessage?: string;
  data: TItem;
}
