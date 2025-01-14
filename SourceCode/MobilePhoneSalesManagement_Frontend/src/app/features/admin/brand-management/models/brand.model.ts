export interface Brand {
    brandId: string;
    name: string;
    imageUrl: string;
    isActive: boolean;
    productCount: number;
    createdAt: Date;
    updatedAt: Date;
}

export interface PagedResult<T> {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalCount: number;
    items: T[];
}