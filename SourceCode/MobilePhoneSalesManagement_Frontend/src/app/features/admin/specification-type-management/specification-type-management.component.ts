import { Component, Input, OnInit } from '@angular/core';
import { SpecificationTypesService } from './services/specification-types.service';
import { Observable } from 'rxjs';
import { specificationType } from './models/specificationType';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-specification-type-management',
  imports: [FormsModule, CommonModule],
  templateUrl: './specification-type-management.component.html',
  styleUrls: ['./specification-type-management.component.css']
})
export class SpecificationTypeManagementComponent implements OnInit {
  @Input() specificationTypes?: Observable<specificationType[]>;

  editMode: boolean[] = [];
  isTemporary: boolean[] = []; // Theo dõi các mục được thêm tạm thời
  newSpecificationName: string = ''; // Thay đổi từ đối tượng thành chuỗi
  isAddingNew: boolean = false;

  constructor(private specificationTypesService: SpecificationTypesService) { }

  ngOnInit(): void {
    if (this.specificationTypes) {
      this.specificationTypes.subscribe(specs => {
        this.editMode = Array(specs.length).fill(false);
        this.isTemporary = Array(specs.length).fill(false);
      });
    }
  }

  load() {
    this.specificationTypes = this.specificationTypesService.getSpecificationTypess();
  }

  toggleEditMode(index: number): void {
    this.editMode[index] = !this.editMode[index];
  }

  startAddingNew() {
    this.isAddingNew = true;
  }

  addSpecification(): void {
    if (this.newSpecificationName.trim()) {
      this.specificationTypesService.addSpecificationTypes({
        "name": this.newSpecificationName
      }).subscribe(() => {
        this.load();
      });
      this.newSpecificationName = ''; // Reset sau khi thêm xong
      this.cancelAddingNew();
    }
  }

  cancelAddingNew() {
    this.isAddingNew = false;
  }

  saveSpecification(index: number, spec: specificationType): void {

    this.editMode[index] = true;
    this.specificationTypesService.updateSpecificationTypes(parseInt(spec.specificationTypeId), {
      "name": spec.name
    }).subscribe(() => {
      // this.editMode[index] = false;
      this.load();
    });
  }

  deleteSpecification(index: number): void {
    if (this.isTemporary[index]) {
      // Nếu là mục tạm thời, chỉ xóa trong danh sách
      this.specificationTypes?.subscribe(specs => {
        specs.splice(index, 1);
        this.editMode.splice(index, 1);
        this.isTemporary.splice(index, 1);
      });
    } else {
      this.specificationTypes?.subscribe(specs => {
        const specToDelete = specs[index];
        this.specificationTypesService.deleteSpecificationTypes(specToDelete.specificationTypeId).subscribe(() => {
          specs.splice(index, 1);
          this.editMode.splice(index, 1);
          this.isTemporary.splice(index, 1);
          this.load();
        });
      });
    }
  }
}
