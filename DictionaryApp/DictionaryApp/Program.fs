open System
open System.Windows.Forms
open System.Drawing
open Newtonsoft.Json
open System.IO

// File paths for saving and loading data
let dictionaryFilePath = "dictionary.json"


// add a word to the dictionary
let addWord (word: string) (definition: string) (dictionary: Map<string, string>) =
    match word.Trim() with
    | "" -> Error "Please enter a valid word."  // Case for empty or whitespace word
    | word when definition.Trim() = "" -> Error "Definition cannot be empty" // Case for empty or whitespace definition
    | word when dictionary.ContainsKey(word.ToLower()) -> Error "Word already exists!"  // Case for existing word
    | word ->  // Case for successfully adding the word
        let updatedDictionary = dictionary.Add(word.ToLower(), definition)
        Ok updatedDictionary


// update a word's definition in the dictionary
let updateWord (word: string) (definition: string) (dictionary: Map<string, string>) =
    match word.Trim() with
    | "" -> Error "Please enter a valid word."  // Case for empty or whitespace word
    | word when definition.Trim() = "" -> Error "Definition cannot be empty" // Case for empty or whitespace definition
    | word when not (dictionary.ContainsKey(word.ToLower())) -> Error "Word not found!"  // Case for word not found
    | word ->  // Case for successfully updating the word
        let updatedDictionary = dictionary.Add(word.ToLower(), definition)
        Ok updatedDictionary
// delete a word from the dictionary
let deleteWord (word: string) (dictionary: Map<string, string>) =
    match word.Trim() with
    | "" -> Error "Please enter a valid word."  // Case for empty or whitespace word
    | word when not (dictionary.ContainsKey(word.ToLower())) -> Error "Word not found!"  // Case for word not found
    | word ->  // Case for successfully deleting the word
        let updatedDictionary = dictionary.Remove(word.ToLower())
        Ok updatedDictionary
// search for words starting with a keyword
let searchWords (keyword: string) (dictionary: Map<string, string>) =
    dictionary
    |> Map.filter (fun key _ -> key.StartsWith(keyword.ToLower()))
    |> Map.toSeq
    |> Seq.map (fun (k, v) -> $"{k}: {v}")
    |> Seq.toArray

// load the dictionary from a JSON file
let loadDictionaryFromFile () =
    try
        if File.Exists(dictionaryFilePath) then
            let json = File.ReadAllText(dictionaryFilePath)
            JsonConvert.DeserializeObject<Map<string, string>>(json)
        else
            Map.empty
    with ex -> 
        MessageBox.Show($"Failed to load dictionary: {ex.Message}") |> ignore
        Map.empty

// Controls
let createControls () =
    // Labels
    let lblWord = new Label(Text = "Enter Word", Location = Point(297, 40), ForeColor = Color.FromArgb(255, 190, 72), Width = 200)
    lblWord.Font <- new Font("Segoe UI Semibold", 14.0f)

    let lblDefinition = new Label(Text = "Enter Definition", Location = Point(279, 100), ForeColor = Color.FromArgb(255, 190, 72), Width = 200)
    lblDefinition.Font <- new Font("Segoe UI Semibold", 14.0f)

    let lblSearch = new Label(Text = "Search Here", Location = Point(200, 247), ForeColor = Color.FromArgb(255, 190, 72), Width = 200)
    lblSearch.Font <- new Font("Segoe UI Semibold", 14.0f)

    // Textboxes
    let txtWord = new TextBox(Location = Point(275, 66), Width = 150, ForeColor = Color.White, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)
    let txtDefinition = new TextBox(Location = Point(150, 126), Width = 400, Height = 60, ForeColor = Color.White, Multiline = true, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)
    let txtSearch = new TextBox(Location = Point(180, 280), Width = 150, ForeColor = Color.White, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)

    // Buttons
    let btnAdd = new Button(Text = "Add", Location = Point(180, 206), Width = 100, ForeColor = Color.White, BackColor = Color.FromArgb(124, 26, 26), FlatStyle = FlatStyle.Flat)
    btnAdd.FlatAppearance.BorderSize <- 0

    let btnUpdate = new Button(Text = "Update", Location = Point(300, 206), Width = 100, ForeColor = Color.White, BackColor = Color.FromArgb(124, 26, 26), FlatStyle = FlatStyle.Flat)
    btnUpdate.FlatAppearance.BorderSize <- 0

    let btnDelete = new Button(Text = "Delete", Location = Point(420, 206), Width = 100, ForeColor = Color.White, BackColor = Color.FromArgb(124, 26, 26), FlatStyle = FlatStyle.Flat)
    btnDelete.FlatAppearance.BorderSize <- 0

    let btnSearch = new Button(Text = "Search", Location = Point(420, 280), Width = 100, ForeColor = Color.White, BackColor = Color.FromArgb(124, 26, 26), FlatStyle = FlatStyle.Flat)
    btnSearch.FlatAppearance.BorderSize <- 0

    // ListBoxes
    let lstSuggestions = new ListBox(Location = Point(180, 297), Width = 150, Height = 75, ForeColor = Color.White, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)
    lstSuggestions.Visible <- false

    let lstResults = new ListBox(Location = Point(100, 330), Width = 500, Height = 80, ForeColor = Color.White, BackColor = Color.FromArgb(23, 30, 46), BorderStyle = BorderStyle.None)

    // Return a tuple with the controls
    (lblWord, txtWord, lblDefinition, txtDefinition, btnAdd, btnUpdate, btnDelete, lblSearch, txtSearch, btnSearch, lstSuggestions, lstResults)


// Form
let createForm () =
    // Create the form
    let form = new Form(Text = "Digital Dictionary", Width = 700, Height = 460, BackColor = Color.FromArgb(18, 24, 36))
    form.Font <- new Font("Segoe UI Semibold", 11.0f)
    form.MaximizeBox <- false
    form.MinimumSize <- form.Size
    form.MaximumSize <- form.Size

    // Retrieve controls as a tuple
    let (lblWord, txtWord, lblDefinition, txtDefinition, btnAdd, btnUpdate, btnDelete, lblSearch, txtSearch, btnSearch, lstSuggestions, lstResults) = createControls ()

    // Add controls to the form
    form.Controls.AddRange([|
        lblWord :> Control; txtWord :> Control; lblDefinition :> Control; txtDefinition :> Control;
        btnAdd :> Control; btnUpdate :> Control; btnDelete :> Control;
        lblSearch :> Control; txtSearch :> Control; btnSearch :> Control;
        lstSuggestions :> Control; lstResults :> Control
    |])

    

    form

[<STAThread>]
let main() =
    let form = createForm ()
    Application.Run(form)

main()
