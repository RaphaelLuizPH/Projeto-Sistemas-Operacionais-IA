export interface GenericResult<T> {
  result?: IActionResult<T> | null;

  isSuccess: boolean;

  error?: number | null;

  message?: string | null;
}

type IActionResult<T> = {
  contentTypes: string[];
  statusCode: number;
  value: T;
  declaredType: string;
} | null;
