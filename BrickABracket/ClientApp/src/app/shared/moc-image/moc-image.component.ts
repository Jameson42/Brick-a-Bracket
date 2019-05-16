import { Component, OnInit, Input } from '@angular/core';
import { MocService } from '@bab/core';

@Component({
  selector: 'app-moc-image',
  templateUrl: './moc-image.component.html',
  styleUrls: ['./moc-image.component.css']
})
export class MocImageComponent implements OnInit {

  @Input()
  set classes(value: string) {
    this._classes = value;
  }

  @Input()
  set mocId(value: string) {
    this._url = this.mocs.getImageUrl(value);
  }

  @Input()
  set alternate(value: File) {
    if (!value) {
      return;
    }
    const reader = new FileReader();
    reader.readAsDataURL(value);
    reader.onload = _ => {
      this._fileUrl = reader.result;
    };
    this._file = value;
  }

  private _classes: string;
  private _url: string;
  private _file: File;
  private _fileUrl: string | ArrayBuffer;

  constructor(private mocs: MocService) { }

  ngOnInit() {
  }

}
