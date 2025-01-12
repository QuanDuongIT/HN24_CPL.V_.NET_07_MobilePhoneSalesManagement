import { Brand } from "../../brand-management/models/brand.model";
import { productSpecifications } from "./specificationType";

export interface Product {
    productId: string;
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
    createdAt: Date;
    updatedAt: Date;
    brand: Brand;
    productSpecifications: productSpecifications[];
}
