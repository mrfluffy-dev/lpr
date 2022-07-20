use prettytable::{Table, Row, Cell};
use std::vec::Vec;
pub fn print_first_table(table: &Vec<Vec<f64>>){
     let mut print_table = Table::new();
    let mut titles: Vec<Cell> = vec![];
    for title in 0..table[0].len(){
        if title == table[1].len() - 1 {
            titles.push(Cell::new("rhs"));
        } else if title < table[0].len() / 2 {
            titles.push(Cell::new(&format!("x{}",title+1)));
        } else {
            titles.push(Cell::new(&format!("s{}",title - table[0].len() / 2 + 1)));
        }
    }
    print_table.add_row(Row::new(titles));
    for i in 0..table.len() {
        let mut cells: Vec<Cell> = vec![];
        for j in 0..table[i].len(){
            cells.push(Cell::new(&table[i][j].to_string()));
        };
        print_table.add_row(Row::new(cells));
    };
    print_table.printstd();

}


pub fn print_table(table: &Vec<Vec<f64>>,pivot_col: &usize, pivot_row: &usize) {
    let mut print_table = Table::new();
    let mut titles: Vec<Cell> = vec![];
    for title in 0..table[0].len(){
        if title == table[0].len() - 1 {
            titles.push(Cell::new("rhs"));
        } else if title < table[0].len() / 2 {
            titles.push(Cell::new(&format!("x{}",title+1)));
        } else {
            titles.push(Cell::new(&format!("s{}",title - table[0].len() / 2 + 1)));
        }
    }
    print_table.add_row(Row::new(titles));
    for i in 0..table.len() {
        let mut cells: Vec<Cell> = vec![];
        for j in 0..table[i].len(){
            cells.push(Cell::new(&table[i][j].to_string()).style_spec(if j == *pivot_col || i == *pivot_row+1{ "Fgb"} else {""}));
        };
        print_table.add_row(Row::new(cells));
    };
    print_table.printstd();
}
