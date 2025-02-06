import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../_services/busy.service';
import { delay, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  // Services
  const busyService = inject(BusyService);

  // Start the loading spinner when the request is about to go out
  busyService.busy();

  return next(req).pipe(
    // This will add artificial delay into the app (make it more realistic with some loading times)
    delay(1000),

    // This will close the spinner once the request is completed
    finalize(() => {
      busyService.idle();
    })
  );
};
