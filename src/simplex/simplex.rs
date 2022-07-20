use std::io::{self, Write};
use std::vec::Vec;
use crate::helpers::table_printers::{print_table, print_first_table};
struct Coll {
    restriction: Vec<f64>,
    sign: String,
    rhs: f64,
}
pub fn simplex() {
    let mut stuff: Vec<String> = ["Red".to_string(),"Green".to_string(),"Blue".to_string()].to_vec();
    stuff.push("Yellow".to_string());
    let amount_restrictions = int_input("Enter amount of restrictions: ");
    let amount_variables = int_input("Enter amount of variables: ");
    let mut max_z: Vec<f64> = vec![];
    for z in 0..amount_variables {
        let input = float_input(&format!("{}x{}:","Enter max value of variable ",z));
        max_z.push(input);
    }
    let all_colls = fill_coll(amount_restrictions, amount_variables);
    let mut table: Vec<Vec<f64>> = vec![];
    //fill in max_z with 0 for the rest of the table till size amount_restrictions
    for _i in 0..amount_restrictions {
        max_z.push(0.0);
    }
    table.push(max_z);

    let mut count = 0;
    for mut coll in all_colls{
        for i in 0..amount_restrictions {
            //push 1 if i = count
            if i == count {
                coll.restriction.push(1.0);
            } else {
                coll.restriction.push(0.0);
            }
        }
        coll.restriction.push(coll.rhs);
        count += 1;
        table.push(coll.restriction);
    }

    println!("t*");
    print_first_table(&table);
    println!("");
    println!("ti");
    let mut table = flip_z(&mut table);
    //print_table(&table);
    //pivot table until table is optimal
    // table is optimal when z has no negative values
    println!("");
    let mut optimal = false;
    while !optimal {
        let pivot_col = find_pivot_col(&table);
        let ratio = ratio_test(&table, pivot_col);
        let pivot_row = find_pivot_row(&ratio);
        print_table(&table,&pivot_col,&pivot_row);
        table = pivot_table(&mut table, pivot_row, pivot_col);
        println!("");
        if check_optimal(&table){
            print_table(&table, &pivot_col, &pivot_row);
            optimal = true;
        }
    }
}

fn check_optimal(table: &Vec<Vec<f64>>) -> bool {
// tebale is optimal when z has no negative values
    let mut z_index = 0;
    for i in 0..table[0].len() {
        if table[0][i] < 0.0 {
            z_index = i;
        }
    }
    if z_index == 0 {
        return true;
    } else {
        return false;
    }
}

//fn print_first_table(table: &Vec<Vec<f64>>){
//     let mut print_table = Table::new();
//    let mut titles: Vec<Cell> = vec![];
//    for title in 0..table[0].len(){
//        if title == table[1].len() - 1 {
//            titles.push(Cell::new("rhs"));
//        } else if title < table[0].len() / 2 {
//            titles.push(Cell::new(&format!("x{}",title+1)));
//        } else {
//            titles.push(Cell::new(&format!("s{}",title - table[0].len() / 2 + 1)));
//        }
//    }
//    print_table.add_row(Row::new(titles));
//    for i in 0..table.len() {
//        let mut cells: Vec<Cell> = vec![];
//        for j in 0..table[i].len(){
//            cells.push(Cell::new(&table[i][j].to_string()));
//        };
//        print_table.add_row(Row::new(cells));
//    };
//    print_table.printstd();
//
//}
//
//
//fn print_table(table: &Vec<Vec<f64>>,pivot_col: &usize, pivot_row: &usize) {
//    let mut print_table = Table::new();
//    let mut titles: Vec<Cell> = vec![];
//    for title in 0..table[0].len(){
//        if title == table[0].len() - 1 {
//            titles.push(Cell::new("rhs"));
//        } else if title < table[0].len() / 2 {
//            titles.push(Cell::new(&format!("x{}",title+1)));
//        } else {
//            titles.push(Cell::new(&format!("s{}",title - table[0].len() / 2 + 1)));
//        }
//    }
//    print_table.add_row(Row::new(titles));
//    for i in 0..table.len() {
//        let mut cells: Vec<Cell> = vec![];
//        for j in 0..table[i].len(){
//            cells.push(Cell::new(&table[i][j].to_string()).style_spec(if j == *pivot_col || i == *pivot_row+1{ "Fgb"} else {""}));
//        };
//        print_table.add_row(Row::new(cells));
//    };
//    print_table.printstd();
//}

fn pivot_table(table: &mut Vec<Vec<f64>>, pivot_row: usize, pivot_col: usize) -> Vec<Vec<f64>> {
    // firnd the pivot point
    let mut new_table = table.clone();
    let pivot_point = table[pivot_row+1][pivot_col];
    // divide pivot row by pivot point
    for i in 0..table[pivot_row+1].len() {
        new_table[pivot_row+1][i] = table[pivot_row+1][i] / pivot_point;
    };
    // current possition-(pivot_row*new_table pivot_point)
    for i in 0..table.len() {
        if i != pivot_row+1 {
            let current_possition = table[i][pivot_col];
            for j in 0..table[i].len() {
                new_table[i][j] = table[i][j] - (current_possition * new_table[pivot_row+1][j]);
            }
        }
    }
    return new_table;
}


fn flip_z(table: &mut Vec<Vec<f64>>) -> Vec<Vec<f64>>{
    for i in 0..table[0].len(){
        if table[0][i] > 0.0 {
            table[0][i] = -table[0][i];
        }
    }
    table[0].push(0.0);
    return table.to_vec();
}

fn find_pivot_col(table: &Vec<Vec<f64>>) -> usize {
    let mut largest = 0.0;
    let mut largest_index = 0;
    for i in 0..table[0].len() {
        if table[0][i] < largest{
            largest = table[0][i];
            largest_index = i;
        }
    }
    return largest_index;
}

fn ratio_test(table: &Vec<Vec<f64>>, test_col: usize) -> Vec<f64> {
    let mut ratios: Vec<f64> = vec![];
    for i in 1..table.len() {
        if table[i][test_col] != 0.0 {
            ratios.push(table[i][table[i].len() - 1] / table[i][test_col]);
        } else {
            ratios.push(std::f64::MAX);
        }
    }
    return ratios;
}

fn find_pivot_row(ratios: &Vec<f64>) -> usize {
    //find the index of the smallest ratio
    let mut smallest = std::f64::MAX;
    let mut smallest_index = 0;
    for i in 0..ratios.len() {
        if ratios[i] < smallest && ratios[i] > 0.0 {
            smallest = ratios[i];
            smallest_index = i;
        }
    }
    return smallest_index;
}


fn fill_coll(amount_restrictions: usize, amount_vars: usize) -> Vec<Coll> {
    let mut colls : Vec<Coll> = vec![];
    for _restriction in 0..amount_restrictions {
        let mut restrictions:Vec<f64> = vec![];
        let mut count = 1;
        for _var in 0..amount_vars {
            let input = float_input(&format!("{}x{}:","Enter restriction ",count));
            restrictions.push(input.clone());
            count += 1;
        }
        let sign_local = string_input("Enter sign: ");
        let rhs_local = float_input("Enter rhs: ");
        let coll_local = Coll {
            restriction: restrictions,
            sign: sign_local,
            rhs: rhs_local,
        };
        colls.push(coll_local);
    }
    colls
}



pub fn string_input(prompt: &str) -> String {
    print!("{}", prompt);
    let mut input = String::new();
    let _ = io::stdout().flush();
    io::stdin()
        .read_line(&mut input)
        .expect("Error reading from STDIN");
    input.trim().to_string()
}

pub fn int_input(prompt: &str) -> usize {
    print!("{}", prompt);
    let mut input = String::new();
    let _ = io::stdout().flush();
    io::stdin()
        .read_line(&mut input)
        .expect("Error reading from STDIN");
    //try to parse the input as usize else return max usize
    match input.trim().parse::<usize>() {
        Ok(i) => i,
        Err(_) => {
            usize::max_value()
        }
    }
}

pub fn float_input(prompt: &str) -> f64 {
    print!("{}", prompt);
    let mut input = String::new();
    let _ = io::stdout().flush();
    io::stdin()
        .read_line(&mut input)
        .expect("Error reading from STDIN");
    //try to parse the input as f64 else return max f64
    let output = input.trim().parse::<f64>().unwrap();
    output
}
