import {
    createColumnHelper,
    flexRender,
    getCoreRowModel,
    getFilteredRowModel,
    getSortedRowModel,
    useReactTable,
} from "@tanstack/react-table";
import type { SortingState } from "@tanstack/react-table";
import type { GameSummaryResponse } from "./types";
import { useQuery } from "@tanstack/react-query";
import { getSummaries } from "./api";
import { useState } from "react";

const columnHelper = createColumnHelper<GameSummaryResponse>();

const columns = [
    columnHelper.accessor("totalShots", {
        header: "Shots",
        cell: (info) => `${info.getValue()} shots`,
    }),
    columnHelper.accessor("shipCount", {
        header: "Ships",
        cell: (info) => `${info.getValue()} ships`,
    }),
    columnHelper.accessor("boardSize", {
        header: "Board",
        cell: (info) => `${info.getValue()}x${info.getValue()}`,
    }),
    columnHelper.accessor("completedAtUtc", {
        header: "Completed",
        cell: (info) => new Date(info.getValue()).toLocaleString(),
    }),
];



function SummaryView() {

    const {
        data: summaries = [],
        isFetching,
        error,
    } = useQuery({
        queryKey: ["summaries"],
        queryFn: getSummaries,
    });

    const [sorting, setSorting] = useState<SortingState>([]);
    const [globalFilter, setGlobalFilter] = useState("");

    const table = useReactTable({
        data: summaries,
        columns,
        state: {
            sorting,
            globalFilter,
        },
        onSortingChange: setSorting,
        onGlobalFilterChange: setGlobalFilter,
        getCoreRowModel: getCoreRowModel(),
        getSortedRowModel: getSortedRowModel(),
        getFilteredRowModel: getFilteredRowModel(),
    });


    if (isFetching) {
        return (<p>Loading ...</p>)
    }

    if (error) {
        return (<p>Error loading data.</p>)
    }

    return (
        <section className="summary-list">
            <div className="summary-header">
                <label className="summary-filter">
                    <span>Filter summaries</span>
                    <input
                        value={globalFilter}
                        onChange={(event) => setGlobalFilter(event.target.value)}
                        placeholder="Search shots, ships, board, or date"
                    />
                </label>
                <table >
                    <thead>
                        {table.getHeaderGroups().map((headerGroup) => (
                            <tr key={headerGroup.id}>
                                {headerGroup.headers.map((header) => (
                                    <th key={header.id}>
                                        {header.isPlaceholder ? null : (
                                            <button
                                                type="button"
                                                className={`summary-sort ${header.column.getIsSorted() ? "active" : ""}`}
                                                onClick={header.column.getToggleSortingHandler()}
                                            >
                                                {flexRender(header.column.columnDef.header, header.getContext())}
                                                <span aria-hidden="true">
                                                    {{
                                                        asc: " ↑",
                                                        desc: " ↓",
                                                    }[header.column.getIsSorted() as string] ?? ""}
                                                </span>
                                            </button>
                                        )}
                                    </th>
                                ))}
                            </tr>
                        ))}
                    </thead>
                    <tbody>
                        {table.getRowModel().rows.length === 0 ? (
                            <tr>
                                <td colSpan={columns.length}>No summaries match your filter.</td>
                            </tr>
                        ) : (
                            table.getRowModel().rows.map((row) => (
                                <tr key={row.id}>
                                    {row.getVisibleCells().map((cell) => (
                                        <td key={cell.id}>
                                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                        </td>
                                    ))}
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </section>
    )
}

export default SummaryView;