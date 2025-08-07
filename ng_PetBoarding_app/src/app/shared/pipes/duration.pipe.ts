import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'duration',
  standalone: true,
  pure: true
})
export class DurationPipe implements PipeTransform {
  transform(minutes: number): string {
    if (minutes >= 1440) {
      const days = Math.floor(minutes / 1440);
      return `${days} jour${days > 1 ? 's' : ''}`;
    } else if (minutes >= 60) {
      const hours = Math.floor(minutes / 60);
      const remainingMinutes = minutes % 60;
      if (remainingMinutes === 0) {
        return `${hours}h`;
      } else {
        return `${hours}h${remainingMinutes}`;
      }
    } else {
      return `${minutes} min`;
    }
  }
}
