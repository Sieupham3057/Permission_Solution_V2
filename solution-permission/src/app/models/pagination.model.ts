export interface PagedResponse<T> {
    value: {
        items: T[];
        pageIndex: number;
        pageSize: number;
        totalCount: number;
        hasNextPage: boolean;
        hasPreviousPage: boolean;
    };
    isSuccess: boolean;
    isFailure: boolean;
    error?: {
        code: string;
        message: string;
    };
}
