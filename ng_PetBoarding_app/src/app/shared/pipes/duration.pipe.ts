import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'duration',
  standalone: true,
  pure: true
})
export class DurationPipe implements PipeTransform {
  transform(minutes: number): string {
    if (!minutes || minutes <= 0) {
      return '0 min';
    }

    const totalMinutes = Math.round(minutes);
    const days = Math.floor(totalMinutes / (24 * 60));
    const hours = Math.floor((totalMinutes % (24 * 60)) / 60);
    const mins = totalMinutes % 60;

    const parts: string[] = [];

    if (days > 0) {
      parts.push(`${days}j`);
    }

    if (hours > 0) {
      parts.push(`${hours}h`);
    }

    if (mins > 0) {
      parts.push(`${mins} min`);
    }

    return parts.join(' ') || '0 min';
  }
}
