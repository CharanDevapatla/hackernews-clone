import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StoryService } from '../services/story.service';
import { Story } from '../models/story.model';

@Component({
  selector: 'app-story-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './story-list.html',
  styleUrl: './story-list.css'
})
export class StoryList implements OnInit {
  stories: Story[] = [];
  searchQuery = '';
  page = 1;
  size = 20;
  totalPages = 0;
  isLoading = false;
  errorMsg = '';

  constructor(private service: StoryService) {}

  ngOnInit() {
    this.fetchStories();
  }

  fetchStories() {
    this.isLoading = true;
    this.errorMsg = '';
    
    this.service.getStories(this.page, this.size, this.searchQuery)
      .subscribe({
        next: (data) => {
          this.stories = data.items;
          this.totalPages = data.totalPages;
          this.isLoading = false;
        },
        error: () => {
          this.errorMsg = 'Could not load stories';
          this.isLoading = false;
        }
      });
  }

  onSearch() {
    this.page = 1;
    this.fetchStories();
  }

  changePage(newPage: number) {
    if (newPage >= 1 && newPage <= this.totalPages) {
      this.page = newPage;
      this.fetchStories();
    }
  }

  getUrl(story: Story): string {
    return story.url || `https://news.ycombinator.com/item?id=${story.id}`;
  }
}
