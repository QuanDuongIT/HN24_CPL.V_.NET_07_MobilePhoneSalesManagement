export interface RequestSpecificationType {
    name: string;
}

export interface RequestProductSpecifications {
    specificationTypeId: string;
    value: string;
    specificationType: RequestSpecificationType;
}


export interface ProductSpecificationWithEditMode extends RequestProductSpecifications {
    editMode: boolean;
}