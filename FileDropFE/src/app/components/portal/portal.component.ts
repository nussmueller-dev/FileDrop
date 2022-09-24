import { Component, HostBinding, HostListener, OnInit } from '@angular/core';

@Component({
  selector: 'app-portal',
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.scss']
})
export class PortalComponent implements OnInit {
  dragOver: boolean = false;
  dragOverPortal: boolean = false;

  @HostBinding('class.drag-over') get dragOverClass() { return this.dragOver; }
  @HostBinding('class.drag-over-portal') get dragOverPortalClass() { return this.dragOverPortal; }

  @HostListener('document:dragover', ['$event']) onDragOver(event: DragEvent) {
    this.dragOver = true;
  }

  @HostListener('document:drop')
  @HostListener('document:dragleave') onDragLeave() {
    this.dragOver = false;
  }

  @HostListener('dragover', ['$event']) onDragOverPortal(event: DragEvent) {
    this.dragOverPortal = true;
  }

  @HostListener('drop')
  @HostListener('dragleave') onDragPortalLeave() {
    this.dragOverPortal = false;
  }

  constructor() { }

  ngOnInit(): void {
  }

}
