using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using posCoreModuleApi.Services;
using Microsoft.Extensions.Options;
using posCoreModuleApi.Configuration;
using posCoreModuleApi.Entities;
using Dapper;
using System.Linq;
using System.Data;
using Npgsql;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace posCoreModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private readonly dapperQuery _dapperQuery;
        private string cmd, cmd2, cmd3, cmd4, cmd5, cmd6;
        public string saveConStr;

        public ProductController(dapperQuery dapperQuery,IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("getProduct")] 
        public IActionResult getProduct(int branchID,int companyID,int userID, int moduleId)
        {
            try
            {   
                if (branchID != 0 && companyID == 0)
                {
                    cmd = "SELECT * FROM view_product where \"branchid\" = " + branchID + " order by \"productID\" desc";
                }
                else
                {
                    cmd = "SELECT * FROM view_product where \"branchid\" = " + branchID + " AND \"companyid\" = " + companyID + " order by \"productID\" desc";
                }
                var appMenu = _dapperQuery.StrConQry<Product>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        // [HttpGet("getTopFKProducts")]
        // public IActionResult getTopFKProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc offset 2500 limit 2500";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        // [HttpGet("getTopTKProducts")]
        // public IActionResult getTopTKProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc offset 1200 limit 2500";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        // [HttpGet("getTopOKProducts")]
        // public IActionResult getTopOKProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc  offset 600 limit 1200";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        // [HttpGet("getTopFHProducts")]
        // public IActionResult getTopFHProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc offset 100 limit 600";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        // [HttpGet("getTopProducts")]
        // public IActionResult getTopProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc limit 100";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        [HttpGet("getProductByCategory")]
        public IActionResult getProductByCategory(int categoryID, int companyID, int branchID,int userID, int moduleId)
        {
            try
            {
                if (branchID != 0 && companyID == 0)
                {
                    cmd = "SELECT * FROM view_product where \"categoryID\" = " + categoryID + " AND \"branchid\" = " + branchID + " order by \"productID\" desc";
                }
                else
                {
                    cmd = "SELECT * FROM view_product where \"categoryID\" = " + categoryID + " AND \"branchid\" = " + branchID + " AND \"companyid\" = " + companyID + " order by \"productID\" desc";    
                }

                var appMenu = _dapperQuery.StrConQry<Product>(cmd,userID,moduleId);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveProduct")]
        public IActionResult saveProduct(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                var response = "";
                List<Product> appMenuProduct = new List<Product>();
                List<Product> appMenuBarcode = new List<Product>();
                var found = false;
                var productName = "";

                cmd2 = "select \"productName\" from product where \"isDeleted\"::int = 0 AND \"productName\" = '" + obj.productName + "' AND \"businessid\" = " + obj.businessid + " AND \"companyid\" = " + obj.companyid + "";
                appMenuProduct = (List<Product>)_dapperQuery.StrConQry<Product>(cmd2, obj.userID,obj.moduleId);

                if (appMenuProduct.Count > 0)
                    productName = appMenuProduct[0].productName;

                // var colorID = "";
                // var sizeID = "";
                // if(obj.colorID == 0){
                //     colorID = "";
                // }else{
                //     colorID = obj.colorID.ToString();
                // }
                // if(obj.sizeID == 0){
                //     sizeID = "";
                // }else{
                //     sizeID = obj.sizeID.ToString();
                // }
                // obj.applicationEDoc
                
                if (productName == "")
                {
                    
                    
                    //  string productIDQuery = "SELECT case when max(\"productID\") is null then 1 else max(\"productID\") + 1 FROM public.product";
                     string productIDQuery = "SELECT COALESCE(MAX(\"productID\"), 0) + 1 FROM public.product";

                     int productID;

                    if (obj.userID != 0 && obj.moduleId != 0)
                    {
                        saveConStr = _dapperQuery.FindMe(obj.userID, obj.moduleId);
                    }

                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    using (NpgsqlCommand command = new NpgsqlCommand(productIDQuery, con))
                    {
                        con.Open();
                        productID = int.Parse(command.ExecuteScalar().ToString());
                    }

                    //cmd = "insert into public.product (\"categoryID\", \"uomID\", \"brandID\", \"productName\", \"productNameUrdu\", \"ROL\", \"maxLimit\", \"quickSale\", \"pctCode\", \"applicationedoc\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values ('" + obj.categoryID + "', " + obj.uomID + ", " + obj.brandID + ", '" + obj.productName + "', '" + obj.productNameUrdu + "', '" + obj.reOrderLevel + "', '" + obj.maxLimit + "', '" + obj.quickSale + "', '" + obj.pctCode + "', '" + obj.applicationEDoc + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + ")";
                    cmd = "insert into public.product (\"productID\",\"categoryID\", \"productName\", \"productNameUrdu\", \"applicationedoc\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"potType\",\"potSize\",\"brandID\",\"uomID\", \"mfgDate\", \"expDate\",\"branchID\") values ('" + productID + "','" + obj.categoryID + "', '" + obj.productName + "', '" + obj.productNameUrdu + "', '" + obj.applicationEDoc + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + ",'" + obj.potType + "','" + obj.potSize + "'," + obj.brandID + ", " + obj.uomID + ", '" + obj.mfgDate + "', '" + obj.expDate + "', " + obj.branchid + ")";
                }
                else
                {
                    found = true;
                }


                if (found == false)
                {
                    
                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected = con.Execute(cmd);
                    }
                }

                if (rowAffected > 0)
                {


                    cmd2 = "SELECT \"productID\" FROM public.product order by \"productID\" desc limit 1";
                    appMenuProduct = (List<Product>)_dapperQuery.StrConQry<Product>(cmd2, obj.userID,obj.moduleId);

                    var prodID = appMenuProduct[0].productID;

                    if (obj.barcode1 == "")
                    {

                        cmd3 = "select \"barcodeID\" from public.barcode order by \"barcodeID\" desc limit 1";
                        appMenuBarcode = (List<Product>)_dapperQuery.StrConQry<Product>(cmd3, obj.userID,obj.moduleId);

                        var barcodeID = 0;
                        if (appMenuBarcode.Count > 0)
                        {
                            barcodeID = appMenuBarcode[0].barcodeID + 1;
                        }
                        else
                        {
                            barcodeID = 1;
                        }

                        

                        cmd4 = "INSERT INTO public.barcode(\"barcodeID\",\"productID\", \"barcode1\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values (" + barcodeID + "," + prodID + ", '" + barcodeID + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + ")";

                    }
                    else
                    {

                        // start for autoincrement of barcodeID
                    string barcodeIDQuery = "SELECT COALESCE(MAX(\"barcodeID\"), 0) + 1 FROM public.barcode";

                     int barcodeID;

                    if (obj.userID != 0 && obj.moduleId != 0)
                    {
                        saveConStr = _dapperQuery.FindMe(obj.userID, obj.moduleId);
                    }

                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    using (NpgsqlCommand command = new NpgsqlCommand(barcodeIDQuery, con))
                    {
                        con.Open();
                        barcodeID = int.Parse(command.ExecuteScalar().ToString());
                    }

                    // end for autoincrement of barcodeID


                        cmd4 = "INSERT INTO public.barcode(\"barcodeID\",\"productID\", \"barcode1\", \"barcode2\", \"barcode3\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values (" + barcodeID + "," + prodID + ", '" + obj.barcode1 + "', '" + obj.barcode2 + "', '" + obj.barcode3 + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessid + "," + obj.companyid + ")";

                    }

                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected2 = con.Execute(cmd4);
                    }

                    // start for autoincrement of productPrice
                    string pPriceIDQuery = "SELECT COALESCE(MAX(\"pPriceID\"), 0) + 1 FROM public.\"productPrice\"";

                     int pPriceID;

                    if (obj.userID != 0 && obj.moduleId != 0)
                    {
                        saveConStr = _dapperQuery.FindMe(obj.userID, obj.moduleId);
                    }

                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    using (NpgsqlCommand command = new NpgsqlCommand(pPriceIDQuery, con))
                    {
                        con.Open();
                        pPriceID = int.Parse(command.ExecuteScalar().ToString());
                    }

                    // end for autoincrement of productPrice


                    cmd5 = "insert into public.\"productPrice\" (\"pPriceID\",\"productID\", \"costPrice\", \"salePrice\", \"retailPrice\", \"wholeSalePrice\", \"gst\", \"et\", \"packing\", \"packingSalePrice\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values (" + pPriceID + "," + prodID + ", " + obj.costPrice + ", " + obj.salePrice + ", " + obj.retailPrice + ", " + obj.wholeSalePrice + ", " + obj.gst + ", " + obj.et + ", " + obj.packingQty + ", " + obj.packingSalePrice + ", '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchid + "," + obj.businessid + "," + obj.companyid + ")";
                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected3 = con.Execute(cmd5);
                    }

                    if (obj.applicationEDocPath != null && obj.applicationEDocPath != "")
                    {
                        dapperQuery.saveImageFile(
                            obj.applicationEDoc,
                            prodID.ToString(),
                            obj.applicationEDocPath,
                            obj.applicationEdocExtenstion);
                    }
                }

                if (rowAffected > 0 && rowAffected2 > 0 && rowAffected3 > 0)
                {
                    response = "Success";
                }
                else
                {
                    if (found == true)
                    {
                        response = "Product name already exist";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("updateProduct")]
        public IActionResult updateProduct(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                var response = "";

                cmd = "update product set \"categoryID\" = '" + obj.categoryID + "',\"brandID\" = '" + obj.brandID + "',\"uomID\" = '" + obj.uomID + "', \"productName\" = '" + obj.productName + "', \"productNameUrdu\" = '" + obj.productNameUrdu + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + ",\"potType\" = '" + obj.potType + "',\"potSize\" = '" + obj.potSize + "',\"applicationedoc\" = '" + obj.applicationEDoc + "', \"mfgDate\" = '" + obj.mfgDate + "', \"expDate\" = '" + obj.expDate + "' where \"productID\" = " + obj.productID + ";";

                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    cmd3 = "update public.barcode set \"barcode1\" = '" + obj.barcode1 + "', \"barcode2\" = '" + obj.barcode2 + "', \"barcode3\" = '" + obj.barcode3 + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"barcodeID\" = " + obj.barcodeID + ";";

                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected2 = con.Execute(cmd3);
                    }

                    cmd4 = "update public.\"productPrice\" set \"costPrice\" = '" + obj.costPrice + "', \"salePrice\" = '" + obj.salePrice + "', \"retailPrice\" = '" + obj.retailPrice + "', \"wholeSalePrice\" = '" + obj.wholeSalePrice + "', \"gst\" = '" + obj.gst + "', \"et\" = '" + obj.et + "', \"packing\" = '" + obj.packingQty + "', \"packingSalePrice\" = '" + obj.packingSalePrice + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"pPriceID\" = " + obj.pPriceID + ";";
                    if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                    using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                    {
                        rowAffected3 = con.Execute(cmd4);
                    }
                }

                if (obj.applicationEDocPath != null && obj.applicationEDocPath != "")
                {
                    dapperQuery.saveImageFile(
                        obj.applicationEDoc,
                        obj.productID.ToString(),
                        obj.applicationEDocPath,
                        obj.applicationEdocExtenstion);
                }
                if (rowAffected > 0 && rowAffected2 > 0 && rowAffected3 > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Invalid Input Error";
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("updateProductPrice")]
        public IActionResult updateProductPrice(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";

                //cmd = "update product set \"brandID\" = " + obj.brandID + " where \"productID\" = " + obj.productID + ";";
                    
                // using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                // {
                //     rowAffected = con.Execute(cmd);
                // }

                cmd2 = "update public.\"productPrice\" set \"costPrice\" = '" + obj.costPrice + "', \"salePrice\" = '" + obj.salePrice + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"productID\" = " + obj.productID + ";";

                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                {
                    rowAffected2 = con.Execute(cmd2);
                }

                if (rowAffected > 0 && rowAffected2 > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Invalid Input Error";
                }
                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("updateProductImage")]
        public IActionResult updateProductImage(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                var response = "";

                cmd = "update product set  \"applicationedoc\" = '" + obj.applicationEDocPath + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"productID\" = " + obj.productID + ";";

                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                    saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                    }
                using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    dapperQuery.saveImageFile(
                        obj.applicationEDocPath,
                        obj.productID.ToString(),
                        obj.applicationEDoc,
                        obj.applicationEdocExtenstion);
                }

                if (rowAffected > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Invalid Input Error";
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("deleteProduct")]
        public IActionResult deleteProduct(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                var response = "";

                cmd = "update product set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"productID\" = " + obj.productID + ";";

                if(obj.userID != 0 && obj.moduleId !=0)
                    {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                        rowAffected = con.Execute(cmd);
                        }
                    }

                if (rowAffected > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Invalid Input Error";
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveProductImport")]
        public IActionResult saveProductImport(ImportProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                int categoryID = 0;
                int newCategoryID = 0;
                int parentCategoryID = 0;
                var response = "";
                var categoryExist = "";
                var subCategoryExist = "";
                // var distinctCategoryName = "";
                List<Product> appMenuProduct = new List<Product>();
                List<Category> appMenuCategory = new List<Category>();
                List<Category> appMenuSubCategory = new List<Category>();
                List<Product> appMenuBarcode = new List<Product>();
                var found = false;
                var productName = "";
                
                var invObject = JsonConvert.DeserializeObject<List<ProductImportCreation>>(obj.json);

                var distinctCategoryName = invObject.Select(p => new{p.product_category}).Distinct().ToList();

                // var distinctCategoryName = invObject
                // .Where(p => p != null && p.product_category != null)
                // .Select(p => p.product_category)
                // .Distinct()
                // .ToList();

                foreach (var item in distinctCategoryName)
                {
                    cmd2 = "select \"categoryName\" from category where \"isDeleted\"::int = 0 AND \"categoryName\" = '" + item.product_category + "' AND \"businessid\" = " + obj.businessID + " AND \"companyid\" = " + obj.companyID + " ";
                    appMenuCategory = (List<Category>)_dapperQuery.StrConQry<Category>(cmd2, obj.userID,obj.moduleId);

                    if (appMenuCategory.Count > 0)
                        categoryExist = appMenuCategory[0].categoryName;

                    List<Category> appMenuCategoryIncreament = new List<Category>();
                    cmd6 = "Select \"categoryID\" from category Order By \"categoryID\" Desc Limit 1 ";
                    appMenuCategoryIncreament = (List<Category>)_dapperQuery.StrConQry<Category>(cmd6, obj.userID,obj.moduleId);

                        newCategoryID = appMenuCategoryIncreament[0].categoryID+1;
                    
                    if (categoryExist == "")
                    {
                        cmd = "insert into public.category (\"categoryID\",\"categoryName\", \"level1\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"branchID\") values ('" + newCategoryID + "','" + item.product_category + "',1, '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessID + "," + obj.companyID + ", " + obj.branchID + ")";

                        if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected = con.Execute(cmd);
                        }
                    }
                }

                var distinctSubCategoryName = invObject.Select(p => new{p.product_category,p.product_sub_category}).Distinct().ToList();

                foreach (var item in distinctSubCategoryName)
                {
                    cmd2 = "Select \"categoryName\" from category where \"isDeleted\"::int = 0 AND \"categoryName\" = '" + item.product_sub_category + "' AND \"businessid\" = " + obj.businessID + " AND \"companyid\" = " + obj.companyID + "";
                    appMenuSubCategory = (List<Category>)_dapperQuery.StrConQry<Category>(cmd2, obj.userID,obj.moduleId);

                    if (appMenuSubCategory.Count > 0)
                        categoryExist = appMenuSubCategory[0].categoryName;

                    List<Category> appMenuCategoryIncreament = new List<Category>();
                    cmd6 = "Select \"categoryID\" from category Order By \"categoryID\" Desc Limit 1 ";
                    appMenuCategoryIncreament = (List<Category>)_dapperQuery.StrConQry<Category>(cmd6, obj.userID,obj.moduleId);

                        newCategoryID = appMenuCategoryIncreament[0].categoryID+1;

                    List<Category> appMenuParentCategory = new List<Category>();
                    cmd6 = "Select \"categoryID\" from category where \"categoryName\" = '" + item.product_sub_category + "' and \"isDeleted\" = B'0' AND \"businessid\" = " + obj.businessID + " AND \"companyid\" = " + obj.companyID + "";
                    appMenuParentCategory = (List<Category>)_dapperQuery.StrConQry<Category>(cmd6, obj.userID,obj.moduleId);

                        parentCategoryID = appMenuParentCategory[0].categoryID;
                    
                    if (categoryExist == "")
                    {
                        cmd = "insert into public.category (\"categoryID\",\"categoryName\",\"parentCategoryID\", \"level1\", \"level2\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"branchID\") values ('" + newCategoryID + "','" + item.product_sub_category + "'," + parentCategoryID + ",1,1, '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessID + "," + obj.companyID + ", " + obj.branchID + ")";

                        if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected = con.Execute(cmd);
                        }
                    }
                }

                foreach (var item in invObject)
                {
                    cmd2 = "select \"productName\" from product where \"isDeleted\"::int = 0 AND \"productName\" = '" + item.product_name + "' AND \"businessid\" = " + obj.businessID + " AND \"companyid\" = " + obj.companyID + "";
                    appMenuProduct = (List<Product>)_dapperQuery.StrConQry<Product>(cmd2, obj.userID,obj.moduleId);

                    if (appMenuProduct.Count > 0)
                        productName = appMenuProduct[0].productName;

                    List<Category> forCategoryID = new List<Category>();
                    cmd6 = "Select \"categoryID\" from category where \"categoryName\" = '" + item.product_sub_category + "' ";
                    forCategoryID = (List<Category>)_dapperQuery.StrConQry<Category>(cmd6,  obj.userID,obj.moduleId);

                    categoryID = forCategoryID[0].categoryID+1;
                    
                    if (productName == "")
                    {
                        string productIDQuery = "SELECT COALESCE(MAX(\"productID\"), 0) + 1 FROM public.product";

                        int productID;

                        if (obj.userID != 0 && obj.moduleId != 0)
                        {
                            saveConStr = _dapperQuery.FindMe(obj.userID, obj.moduleId);
                        }

                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        using (NpgsqlCommand command = new NpgsqlCommand(productIDQuery, con))
                        {
                            con.Open();
                            productID = int.Parse(command.ExecuteScalar().ToString());
                        }
                        cmd = "insert into public.product (\"productID\",\"categoryID\", \"productName\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\",\"branchID\") values ('" + productID + "','" + categoryID + "', '" + item.product_name + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessID + "," + obj.companyID + ", " + obj.branchID + ")";
                    }
                    else
                    {
                        found = true;
                    }


                    if (found == false)
                    {
                        
                        if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected = con.Execute(cmd);
                        }
                    }

                    if (rowAffected > 0)
                    {
                        cmd2 = "SELECT \"productID\" FROM public.product order by \"productID\" desc limit 1";
                        appMenuProduct = (List<Product>)_dapperQuery.StrConQry<Product>(cmd2, obj.userID,obj.moduleId);

                        var prodID = appMenuProduct[0].productID;

                        if (item.product_barcode == "")
                        {

                            cmd3 = "select \"barcodeID\" from public.barcode order by \"barcodeID\" desc limit 1";
                            appMenuBarcode = (List<Product>)_dapperQuery.StrConQry<Product>(cmd3, obj.userID,obj.moduleId);

                            var barcodeID = 0;
                            if (appMenuBarcode.Count > 0)
                            {
                                barcodeID = appMenuBarcode[0].barcodeID + 1;
                            }
                            else
                            {
                                barcodeID = 1;
                            }

                            cmd4 = "INSERT INTO public.barcode(\"barcodeID\",\"productID\", \"barcode1\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values (" + barcodeID + "," + prodID + ", '" + barcodeID + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessID + "," + obj.companyID + ")";

                        }
                        else
                        {
                        string barcodeIDQuery = "SELECT COALESCE(MAX(\"barcodeID\"), 0) + 1 FROM public.barcode";

                        int barcodeID;

                        if (obj.userID != 0 && obj.moduleId != 0)
                        {
                            saveConStr = _dapperQuery.FindMe(obj.userID, obj.moduleId);
                        }

                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        using (NpgsqlCommand command = new NpgsqlCommand(barcodeIDQuery, con))
                        {
                            con.Open();
                            barcodeID = int.Parse(command.ExecuteScalar().ToString());
                        }

                        // end for autoincrement of barcodeID
                        cmd4 = "INSERT INTO public.barcode(\"barcodeID\",\"productID\", \"barcode1\", \"createdOn\", \"createdBy\", \"isDeleted\",\"businessid\",\"companyid\") values (" + barcodeID + "," + prodID + ", '" + item.product_barcode + "', '" + curDate + "', " + obj.userID + ", B'0'," + obj.businessID + "," + obj.companyID + ")";

                        }

                        if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected2 = con.Execute(cmd4);
                        }

                        // start for autoincrement of productPrice
                        string pPriceIDQuery = "SELECT COALESCE(MAX(\"pPriceID\"), 0) + 1 FROM public.\"productPrice\"";

                        int pPriceID;

                        if (obj.userID != 0 && obj.moduleId != 0)
                        {
                            saveConStr = _dapperQuery.FindMe(obj.userID, obj.moduleId);
                        }

                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        using (NpgsqlCommand command = new NpgsqlCommand(pPriceIDQuery, con))
                        {
                            con.Open();
                            pPriceID = int.Parse(command.ExecuteScalar().ToString());
                        }

                        // end for autoincrement of productPrice
                        cmd5 = "insert into public.\"productPrice\" (\"pPriceID\",\"productID\", \"costPrice\", \"salePrice\", \"retailPrice\", \"wholeSalePrice\", \"createdOn\", \"createdBy\", \"isDeleted\",\"branchid\",\"businessid\",\"companyid\") values (" + pPriceID + "," + prodID + ", " + item.cost_price + ", " + item.sale_price + ", " + item.sale_price + ", " + item.cost_price + ", '" + curDate + "', " + obj.userID + ", B'0'," + obj.branchID + "," + obj.businessID + "," + obj.companyID + ")";
                        if(obj.userID != 0 && obj.moduleId !=0)
                        {
                        saveConStr = _dapperQuery.FindMe(obj.userID,obj.moduleId);
                        }
                        using (NpgsqlConnection con = new NpgsqlConnection(saveConStr))
                        {
                            rowAffected3 = con.Execute(cmd5);
                        }

                        // if (obj.applicationEDocPath != null && obj.applicationEDocPath != "")
                        // {
                        //     dapperQuery.saveImageFile(
                        //         obj.applicationEDoc,
                        //         prodID.ToString(),
                        //         obj.applicationEDocPath,
                        //         obj.applicationEdocExtenstion);
                        // }
                    }
                }

                if (rowAffected > 0 && rowAffected2 > 0 && rowAffected3 > 0)
                {
                    response = "Success";
                }
                else
                {
                    if (found == true)
                    {
                        response = "Product name already exist";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }
    }
}