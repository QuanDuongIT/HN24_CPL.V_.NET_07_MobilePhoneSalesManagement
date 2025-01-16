import { RequestProductSpecifications } from "../../specification-type-management/models/add-specificationType-request";

export interface RequestProduct {
    name: string;
    description: string;
    price: number;
    oldPrice: number;
    stockQuantity: number;
    brandId: number;
    imageUrl: string;
    manufacturer: string;
    isActive: boolean;
    colors: string;
    discount: number;
    productSpecifications: RequestProductSpecifications[];
}
