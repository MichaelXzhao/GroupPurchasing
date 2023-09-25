let addqty = (qty)=>{
    qty = qty + 1;
    return qty
}

let reduceqty = (qty)=>{
    qty = qty - 1;
    return qty
}

export default {addqty, reduceqty}