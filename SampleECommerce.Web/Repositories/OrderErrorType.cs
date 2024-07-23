namespace SampleECommerce.Web.Repositories;

public enum OrderErrorType
{
    None = 0,
    OrderItemQuantityEqualToOrLessThanZero,
    ProductQuantityLessThanOrderQuantity,
    UserBalanceLessThanSumOfOrder
}
