import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { StoryList } from './story-list/story-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, StoryList],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Hacker News Feed');
}
