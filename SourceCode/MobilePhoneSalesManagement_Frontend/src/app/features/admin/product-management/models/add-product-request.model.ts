import { RequestProductSpecifications } from "./add-specificationType-request";

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
    color: string;
    discount: number;
    productSpecifications: RequestProductSpecifications[];
}
