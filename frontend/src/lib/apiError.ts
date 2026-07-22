import { isAxiosError } from 'axios'

// The API returns errors in one of two shapes: business-rule failures as { error }
// (AuthController/FacilitiesController's Result.Failure pattern), validation/domain
// failures as ProblemDetails { title } (ExceptionHandlingMiddleware). Callers just want a message.
export function getApiErrorMessage(err: unknown, fallback: string): string {
  if (!isAxiosError(err)) return fallback
  return err.response?.data?.error ?? err.response?.data?.title ?? fallback
}
