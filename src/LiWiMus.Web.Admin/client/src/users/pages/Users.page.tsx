import React, { useEffect, useRef, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
  DataGrid,
  GridColDef,
  GridRowsProp,
  GridSelectionModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { Button, Fab } from "@mui/material";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import AddIcon from "@mui/icons-material/Add";
import FiltersPopover from "../../shared/components/FiltersPopover/FiltersPopover";
import FilterListIcon from "@mui/icons-material/FilterList";
import { FilterOptions } from "../../shared/types/FilterOptions";
import { User } from "../types/User";
import DateFormat from "date-fns/format";
import { useUserService } from "../UserService.hook";

type FilterItem = {
  id: number;
  filterColumn: string;
  filterOperator: string;
  filterValue: string | number;
};

interface FilterModel extends Array<FilterItem> {}

export default function UsersPage() {
  const userService = useUserService();

  const [rowsCount, setRowsCount] = useState<number>();
  const [rows, setRows] = useState<GridRowsProp>([]);
  const [page, setPage] = useState(0);
  const [limitItems, setLimitItems] = useState(5);
  const [loading, setLoading] = useState<boolean>(false);
  const [selectionModel, setSelectionModel] = useState<GridSelectionModel>([]);
  const prevSelectionModel = useRef<GridSelectionModel>(selectionModel);
  const [sortModel, setSortModel] = useState<GridSortModel>([
    { field: "id", sort: "asc" },
  ]);
  const handleSortModelChange = (newModel: GridSortModel) => {
    setSortModel(newModel);
  };
  const navigate = useNavigate();
  const columns: GridColDef[] = [
    { field: "id", hide: true },
    {
      field: "userName",
      headerName: "Username",
      flex: 0.5,
      filterable: false,
    },
    {
      field: "email",
      headerName: "Email",
      flex: 0.9,
      filterable: false,
    },
    {
      field: "birthDate",
      headerName: "BirthDate",
      flex: 0.5,
      filterable: false,
      valueFormatter: (params) => {
        if (params.value) return DateFormat(params.value, "dd.MM.yyyy");
      },
    },
    {
      field: "gender",
      headerName: "Gender",
      flex: 0.3,
      filterable: false,
    },
    {
      field: "emailConfirmed",
      headerName: "Email Confirmed",
      flex: 0.4,
      type: "boolean",
      filterable: false,
    },
    /*{
            field: "balance",
            headerName: "Balance",
            flex:0.4,
            type:"number",
            filterable: false,
/!*
            valueGetter: (params) => params.row.user.balance
*!/
        },*/
    {
      field: "edit",
      headerName: "Edit",
      sortable: false,
      flex: 0.5,
      filterable: false,
      renderCell: (params) => {
        const onClick = (e: any) => {
          e.stopPropagation();
          const path = "/admin/users/" + params.getValue(params.id, "id");
          navigate(path);
        };

        return (
          <Button
            variant="contained"
            endIcon={<ArrowForwardIcon />}
            sx={{ borderRadius: "20px", px: 4 }}
            onClick={onClick}
          >
            Edit
          </Button>
        );
      },
    },
  ];
  const [anchorEl, setAnchorEl] = React.useState<HTMLButtonElement | null>(
    null
  );

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };
  const open = Boolean(anchorEl);
  const id = open ? "simple-popover" : undefined;
  const filterOperators: { key: string; value: string }[] = [
    { key: "eq", value: "Equals" },
    { key: "cnt", value: "Contains" },
    { key: "gt", value: "GreaterThan" },
    { key: "ls", value: "LessThan" },
    { key: "sw", value: "StartsWith" },
  ];
  const columnsOptions: any[] = columns
    .filter(
      (name) =>
        name.headerName !== undefined &&
        name.headerName !== "Edit" &&
        name.headerName !== "Email Confirmed" &&
        name.headerName !== "Phone Confirmed"
    )
    .map((opt) => opt.headerName);
  const operatorsOptions: string[] = filterOperators.map((a) => a.value);
  const filterModelInitialState = columnsOptions.map((column, index) => ({
    id: index,
    filterColumn: column,
    filterOperator: "",
    filterValue: "",
  }));
  const [filterModel, setFilterModel] = useState<FilterModel>(
    filterModelInitialState
  );

  const onOperatorChange = (e: any) => {
    setFilterModel((prevState) =>
      prevState.map((el) =>
        el.id.toString() === e.target.id
          ? {
              ...el,
              filterOperator: e.target.value,
            }
          : el
      )
    );
  };

  const onFilterValueChange = (e: any) => {
    setFilterModel((prevState) =>
      prevState.map((el) =>
        el.id.toString() === e.target.id
          ? {
              ...el,
              filterValue: e.target.value,
            }
          : el
      )
    );
  };

  useEffect(() => {
    let active = true;

    (async () => {
      setLoading(true);
      if (sortModel.length === 0) {
        setSortModel([{ field: "id", sort: "asc" }]);
      }
      const order = sortModel[0].sort === "asc" ? "ASC" : "DESC";
      const testColumn: keyof User = sortModel[0].field as any;
      const filters = filterModel
        .filter((a) => a.filterOperator !== "" && a.filterValue !== "")
        .map((a) => ({
          columnName: columns.filter(
            (col) => col.headerName === a.filterColumn
          )[0].field as any,
          operator: filterOperators.filter(
            (op) => op.value === a.filterOperator
          )[0].key as any,
          value: a.filterValue,
        }));
      const options: FilterOptions<User> = {
        filters: filters,
        page: { pageNumber: page + 1, numberOfElementsOnPage: limitItems },
        sorting: [{ columnName: testColumn, order: order }],
      };
      const res = await userService.getUsers(options);

      if (!active) {
        return;
      }
      setRowsCount(res.totalItems);
      setRows(res.data);
      setLoading(false);
      setTimeout(() => {
        setSelectionModel(prevSelectionModel.current);
      });
    })();
    // @ts-ignore

    return () => {
      active = false;
    };
  }, [page, limitItems, sortModel, filterModel]);

  return (
    <div>
      <div>
        <h1>Users page</h1>
      </div>
      <div>
        <Button
          aria-describedby={id}
          onClick={handleClick}
          variant="contained"
          sx={{ borderRadius: "20px", marginBottom: "10px", px: 4 }}
          startIcon={<FilterListIcon />}
        >
          Filters{" "}
        </Button>
        <FiltersPopover
          id={id}
          open={open}
          anchorEl={anchorEl}
          onClose={handleClose}
          columnsOptions={columnsOptions}
          operatorsOptions={operatorsOptions}
          onOperatorChange={onOperatorChange}
          onFilterValueChange={onFilterValueChange}
          inputFilterValues={filterModel.map((el) => el.filterValue)}
          inputFilterOperators={filterModel.map((el) => el.filterOperator)}
        />
      </div>
      <div>
        <DataGrid
          columns={columns}
          rows={rows}
          disableColumnMenu={true}
          pagination
          pageSize={limitItems}
          onPageSizeChange={(newPageSize) => setLimitItems(newPageSize)}
          rowsPerPageOptions={[2, 5, 10, 20]}
          rowCount={rowsCount}
          sortingMode="server"
          sortModel={sortModel}
          onSortModelChange={handleSortModelChange}
          paginationMode="server"
          onPageChange={(newPage) => {
            prevSelectionModel.current = selectionModel;
            setPage(newPage);
          }}
          onSelectionModelChange={(newSelectionModel) => {
            setSelectionModel(newSelectionModel);
          }}
          disableSelectionOnClick={true}
          selectionModel={selectionModel}
          loading={loading}
          autoHeight
        ></DataGrid>
      </div>
      <div>
        <Fab
          color="primary"
          sx={{ position: "fixed", bottom: 50, right: 50 }}
          aria-label="add"
          component={Link}
          to={"/admin/users/create"}
        >
          <AddIcon />
        </Fab>
      </div>
    </div>
  );
}
