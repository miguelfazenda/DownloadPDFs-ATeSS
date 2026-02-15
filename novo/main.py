"""
Thinker GUI with list of empresas with cehckboxes on the left, then a list of declaracoes with checkboxes on the right, then a button to download the selected declaracoes for the selected empresas. The button should call a function that takes the selected empresas and declaracoes and calls the appropriate download functions from downloader.py with the appropriate arguments. The download functions should be called with a progress callback that updates a progress bar in the GUI. The GUI should also have a log area that shows the logs from the download functions. The GUI should be responsive and not freeze during downloads. The GUI should also have a settings area where the user can enter their master password for decryption of passwords in dados.py, and a button to save the master password. The master password should be used to decrypt the passwords in dados.py when loading the empresas. The GUI should also have error handling for any exceptions that occur during downloads or decryption, and show an error message in the log area.

"""

import datetime
import json
import os
from dados import Dados
from tkinter import Tk, StringVar, IntVar, messagebox
from tkinter import filedialog
import tkinter as tk
from tkinter import ttk
import dl
from downloader_excel_wintouch import DownloaderWintouch
import browser
from scraper_recibos_verdes import TipoReciboVerdePrestOUAdquir
from dl import Declaracao
from dados import Empresa

# Example: Add your declarations here (expand as needed)
# Declaracao("DMR - Comprovativo", "Mensal")
# Declaracao("DMR - Doc. Pagamento", "Mensal")
# Declaracao("Retenções", "Mensal")
# Declaracao("Extrato de remunerações", "Mensal")
# Declaracao("Fundos de comp. - Doc. Pagamento", "Mensal")
# Declaracao("R. Verdes Emitidos - PDFs", "Mensal")
Declaracao(
    "R. Verdes Emitidos - Wintouch",
    "Mensal",
    download_function_mensal=lambda driver, ano, mes, empresa, folder_tipo_declaracao, download_folder, gen_novo_nome_ficheiro, estrutura_nomes_ficheiros: DownloaderWintouch.download_recibos_verdes_wintouch(
        driver,
        ano,
        mes,
        TipoReciboVerdePrestOUAdquir.Prestador,
        empresa,
        folder_tipo_declaracao,
        download_folder,
        gen_novo_nome_ficheiro,
        estrutura_nomes_ficheiros,
    ),
)
Declaracao(
    "R. Verdes Adquiridos - Wintouch",
    "Mensal",
    download_function_mensal=lambda driver, ano, mes, empresa, folder_tipo_declaracao, download_folder, gen_novo_nome_ficheiro, estrutura_nomes_ficheiros: DownloaderWintouch.download_recibos_verdes_wintouch(
        driver,
        ano,
        mes,
        TipoReciboVerdePrestOUAdquir.Adquirente,
        empresa,
        folder_tipo_declaracao,
        download_folder,
        gen_novo_nome_ficheiro,
        estrutura_nomes_ficheiros,
    ),
)
# Declaracao("IRS - Modelo 3", "Anual")
# Declaracao("Modelo 22", "Anual")
# Declaracao("IES", "Anual")
# Declaracao("IVA", "Anual")
# Declaracao("R. Verdes Emitidos - Excel", "Anual")
# Declaracao("Certidão AT", "Pedido")
# Declaracao("SS Pedir certidão", "Pedido")
# Declaracao("SS Tranferir última certidão", "Pedido")


class MainWindow:
    SETTINGS_FILE = os.path.abspath(
        os.path.join(os.path.dirname(__file__), ".env/gui_settings.json")
    )

    def __init__(self, master):
        self.master = master
        master.title("Download PDFs AT e SS")

        # Create main container frames
        self.empresas_frame = ttk.Frame(master, width=250)
        self.empresas_frame.pack(side=tk.LEFT, fill=tk.BOTH, expand=False)
        self.empresas_frame.pack_propagate(False)

        self.middle_frame = ttk.Frame(master)
        self.middle_frame.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)

        self.declaracoes_frame = ttk.Frame(master, width=250)
        self.declaracoes_frame.pack(side=tk.LEFT, fill=tk.BOTH, expand=False)
        self.declaracoes_frame.pack_propagate(False)

        self.log_frame = ttk.Frame(self.middle_frame)
        self.log_frame.pack(side=tk.BOTTOM, fill=tk.BOTH, expand=True)

        # Header with label and add button
        empresas_header = ttk.Frame(self.empresas_frame)
        empresas_header.pack(fill=tk.X)

        self.empresas_label = ttk.Label(empresas_header, text="Empresas")
        self.empresas_label.pack(side=tk.LEFT, padx=5)

        self.add_empresa_button = ttk.Button(
            empresas_header, text="+", width=3, command=self.add_empresa
        )
        self.add_empresa_button.pack(side=tk.RIGHT, padx=5)

        # Scrollable frame for empresas checkboxes
        self.empresas_scrollbar = ttk.Scrollbar(self.empresas_frame, orient="vertical")
        self.empresas_scrollbar.pack(side=tk.RIGHT, fill=tk.Y)

        self.empresas_canvas = tk.Canvas(
            self.empresas_frame,
            yscrollcommand=self.empresas_scrollbar.set,
            highlightthickness=0,
        )
        self.empresas_canvas.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)

        self.empresas_scrollbar.config(command=self.empresas_canvas.yview)

        self.empresas_scrollable_frame = ttk.Frame(self.empresas_canvas)
        self.empresas_canvas_window = self.empresas_canvas.create_window(
            (0, 0), window=self.empresas_scrollable_frame, anchor="nw"
        )

        def empresas_configure(event):
            self.empresas_canvas.configure(
                scrollregion=self.empresas_canvas.bbox("all")
            )
            # Update canvas window width to match canvas
            canvas_width = event.width
            self.empresas_canvas.itemconfig(
                self.empresas_canvas_window, width=canvas_width
            )

        self.empresas_scrollable_frame.bind("<Configure>", empresas_configure)
        self.empresas_canvas.bind("<Configure>", empresas_configure)

        # Enable mouse wheel scrolling for empresas
        def on_mousewheel_empresas(event):
            # Handle both Windows/Linux and macOS
            if event.delta:
                self.empresas_canvas.yview_scroll(
                    int(-1 * (event.delta / 120)), "units"
                )
            else:
                # For macOS
                if event.num == 4:
                    self.empresas_canvas.yview_scroll(-1, "units")
                elif event.num == 5:
                    self.empresas_canvas.yview_scroll(1, "units")

        def on_enter_empresas(event):
            # macOS and Windows
            self.empresas_canvas.bind_all("<MouseWheel>", on_mousewheel_empresas)
            # Linux
            self.empresas_canvas.bind_all("<Button-4>", on_mousewheel_empresas)
            self.empresas_canvas.bind_all("<Button-5>", on_mousewheel_empresas)

        def on_leave_empresas(event):
            self.empresas_canvas.unbind_all("<MouseWheel>")
            self.empresas_canvas.unbind_all("<Button-4>")
            self.empresas_canvas.unbind_all("<Button-5>")

        self.empresas_canvas.bind("<Enter>", on_enter_empresas)
        self.empresas_canvas.bind("<Leave>", on_leave_empresas)

        self.declaracoes_label = ttk.Label(self.declaracoes_frame, text="Declarações")
        self.declaracoes_label.pack()

        # Scrollable frame for declaracoes checkboxes
        self.declaracoes_scrollbar = ttk.Scrollbar(
            self.declaracoes_frame, orient="vertical"
        )
        self.declaracoes_scrollbar.pack(side=tk.RIGHT, fill=tk.Y)

        self.declaracoes_canvas = tk.Canvas(
            self.declaracoes_frame,
            yscrollcommand=self.declaracoes_scrollbar.set,
            highlightthickness=0,
        )
        self.declaracoes_canvas.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)

        self.declaracoes_scrollbar.config(command=self.declaracoes_canvas.yview)

        self.declaracoes_scrollable_frame = ttk.Frame(self.declaracoes_canvas)
        self.declaracoes_canvas_window = self.declaracoes_canvas.create_window(
            (0, 0), window=self.declaracoes_scrollable_frame, anchor="nw"
        )

        def declaracoes_configure(event):
            self.declaracoes_canvas.configure(
                scrollregion=self.declaracoes_canvas.bbox("all")
            )
            # Update canvas window width to match canvas
            canvas_width = event.width
            self.declaracoes_canvas.itemconfig(
                self.declaracoes_canvas_window, width=canvas_width
            )

        self.declaracoes_scrollable_frame.bind("<Configure>", declaracoes_configure)
        self.declaracoes_canvas.bind("<Configure>", declaracoes_configure)

        # Enable mouse wheel scrolling for declaracoes
        def on_mousewheel_declaracoes(event):
            # Handle both Windows/Linux and macOS
            if event.delta:
                self.declaracoes_canvas.yview_scroll(
                    int(-1 * (event.delta / 120)), "units"
                )
            else:
                # For macOS
                if event.num == 4:
                    self.declaracoes_canvas.yview_scroll(-1, "units")
                elif event.num == 5:
                    self.declaracoes_canvas.yview_scroll(1, "units")

        def on_enter_declaracoes(event):
            # macOS and Windows
            self.declaracoes_canvas.bind_all("<MouseWheel>", on_mousewheel_declaracoes)
            # Linux
            self.declaracoes_canvas.bind_all("<Button-4>", on_mousewheel_declaracoes)
            self.declaracoes_canvas.bind_all("<Button-5>", on_mousewheel_declaracoes)

        def on_leave_declaracoes(event):
            self.declaracoes_canvas.unbind_all("<MouseWheel>")
            self.declaracoes_canvas.unbind_all("<Button-4>")
            self.declaracoes_canvas.unbind_all("<Button-5>")

        self.declaracoes_canvas.bind("<Enter>", on_enter_declaracoes)
        self.declaracoes_canvas.bind("<Leave>", on_leave_declaracoes)

        # Lists to store checkbox variables
        self.empresas_vars = []
        self.declaracoes_vars = []

        # Controls frame at top of middle pane
        self.controls_frame = ttk.Frame(self.middle_frame)
        self.controls_frame.pack(side=tk.TOP, fill=tk.X, padx=10, pady=10)

        self.date_group = ttk.Frame(self.controls_frame)
        self.date_group.pack(pady=5)

        self.mes_label = ttk.Label(self.date_group, text="Mês:")
        self.mes_label.pack(side=tk.LEFT)
        self.meses = [
            ("Janeiro", 1),
            ("Fevereiro", 2),
            ("Março", 3),
            ("Abril", 4),
            ("Maio", 5),
            ("Junho", 6),
            ("Julho", 7),
            ("Agosto", 8),
            ("Setembro", 9),
            ("Outubro", 10),
            ("Novembro", 11),
            ("Dezembro", 12),
        ]
        self.mes_names = [m[0] for m in self.meses]
        self.mes_map = {m[0]: m[1] for m in self.meses}
        self.mes_combobox = ttk.Combobox(
            self.date_group, values=self.mes_names, state="readonly", width=12
        )
        self.mes_combobox.pack(side=tk.LEFT, padx=5)
        self.ano_label = ttk.Label(self.date_group, text="Ano:")
        self.ano_label.pack(side=tk.LEFT)
        self.ano_entry = ttk.Entry(self.date_group, width=10)
        self.ano_entry.pack(side=tk.LEFT, padx=5)

        # Download folder selection
        self.download_folder_group = ttk.Frame(self.controls_frame)
        self.download_folder_group.pack(pady=5, fill=tk.X)

        self.download_folder_label = ttk.Label(
            self.download_folder_group, text="Diretoría:"
        )
        self.download_folder_label.pack(side=tk.LEFT)
        self.download_folder_entry = ttk.Entry(self.download_folder_group, width=20)
        self.download_folder_entry.pack(side=tk.LEFT, padx=5, fill=tk.X, expand=True)
        self.browse_button = ttk.Button(
            self.download_folder_group, text="...", command=self.browse_download_folder
        )
        self.browse_button.pack(side=tk.LEFT)

        self.executar_button = ttk.Button(
            self.controls_frame, text="Executar", command=self.executar_clicked
        )
        self.executar_button.pack(pady=10)

        # Load settings and populate fields
        self.load_settings()
        # If no month is set, default to current month
        if not self.mes_combobox.get():
            current_month = datetime.datetime.now().month
            for nome, num in self.meses:
                if num == current_month:
                    self.mes_combobox.set(nome)
                    break

        # Create checkboxes for empresas with context menu
        self.empresa_checkboxes = []
        for idx, empresa in enumerate(Dados.empresas):
            var = IntVar()
            checkbox = ttk.Checkbutton(
                self.empresas_scrollable_frame, text=empresa.nome, variable=var
            )
            checkbox.pack(anchor=tk.W)
            self.empresas_vars.append(var)
            self.empresa_checkboxes.append(checkbox)

            # Add right-click context menu
            menu = tk.Menu(self.empresas_scrollable_frame, tearoff=0)
            menu.add_command(
                label="Abrir AT",
                command=lambda e=empresa: browser.Browser.abre_portal_das_financas(e),
            )
            menu.add_command(
                label="Abrir e-Fatura",
                command=lambda e=empresa: browser.Browser.abre_efatura(e),
            )
            menu.add_command(
                label="Abrir Segurança Social",
                command=lambda e=empresa: browser.Browser.abre_seguranca_social(e),
            )
            menu.add_command(
                label="Abrir Fundos de Compensação",
                command=lambda e=empresa: browser.Browser.abre_fundos_de_compensacao(e),
            )
            menu.add_separator()
            menu.add_command(
                label="Editar",
                command=lambda e=empresa, i=idx: self.edit_empresa(e, i),
            )
            menu.add_command(
                label="Remover",
                command=lambda e=empresa: self.remove_empresa(e),
            )

            def show_menu(event, m=menu, cb=checkbox):
                m.tk_popup(event.x_root, event.y_root)

            checkbox.bind("<Button-3>", show_menu)

        # Create checkboxes for declaracoes
        for declaracao in Declaracao.declaracoes:
            var = IntVar()
            checkbox = ttk.Checkbutton(
                self.declaracoes_scrollable_frame, text=str(declaracao), variable=var
            )
            checkbox.pack(anchor=tk.W)
            self.declaracoes_vars.append(var)

    def load_settings(self):
        """Load previous settings from JSON file"""
        try:
            if os.path.exists(self.SETTINGS_FILE):
                with open(self.SETTINGS_FILE, "r", encoding="utf-8") as f:
                    settings = json.load(f)
                    self.download_folder_entry.insert(
                        0, settings.get("download_path", "./downloads")
                    )
                    self.ano_entry.insert(
                        0, settings.get("ano", str(datetime.datetime.now().year))
                    )
                    mes_num = int(
                        settings.get("mes", str(datetime.datetime.now().month))
                    )
                    for nome, num in self.meses:
                        if num == mes_num:
                            self.mes_combobox.set(nome)
                            break
            else:
                # Default values
                date = datetime.datetime.now()
                self.download_folder_entry.insert(0, "./downloads")
                self.ano_entry.insert(0, str(date.year))
                for nome, num in self.meses:
                    if num == date.month:
                        self.mes_combobox.set(nome)
                        break
        except Exception as ex:
            print(f"Erro ao carregar configurações: {ex}")
            date = datetime.datetime.now()
            self.download_folder_entry.insert(0, "./downloads")
            self.ano_entry.insert(0, str(date.year))
            for nome, num in self.meses:
                if num == date.month:
                    self.mes_combobox.set(nome)
                    break

    def save_settings(self):
        """Save current settings to JSON file"""
        try:
            mes_nome = self.mes_combobox.get()
            mes_num = self.mes_map.get(mes_nome, datetime.datetime.now().month)
            settings = {
                "download_path": self.download_folder_entry.get(),
                "ano": self.ano_entry.get(),
                "mes": mes_num,
            }
            with open(self.SETTINGS_FILE, "w", encoding="utf-8") as f:
                json.dump(settings, f, indent=2, ensure_ascii=False)
        except Exception as ex:
            print(f"Erro ao salvar configurações: {ex}")

    def browse_download_folder(self):
        """Open folder selection dialog"""
        folder = filedialog.askdirectory(title="Selecionar pasta de downloads")
        if folder:
            self.download_folder_entry.delete(0, tk.END)
            self.download_folder_entry.insert(0, folder)
            self.save_settings()

    def executar_clicked(self):
        # Get selected empresas from checkboxes
        selected_empresas = [
            Dados.empresas[i]
            for i, var in enumerate(self.empresas_vars)
            if var.get() == 1
        ]
        # Get selected declaracoes from checkboxes
        selected_declaracoes = [
            Declaracao.declaracoes[i]
            for i, var in enumerate(self.declaracoes_vars)
            if var.get() == 1
        ]

        # Save settings before downloading
        self.save_settings()

        # Get download folder from entry
        download_folder = self.download_folder_entry.get() or "./downloads"

        # Get month number from combobox
        mes_nome = self.mes_combobox.get()
        mes_num = self.mes_map.get(mes_nome, datetime.datetime.now().month)

        # Call the appropriate download functions based on selected declaracoes
        dl.Downloader.executar(
            selected_empresas,
            selected_declaracoes,
            self.ano_entry.get(),
            mes_num,
            download_folder,
            report_progress=self.update_progress,
        )
        print("Selected Empresas:", selected_empresas)
        print("Selected Declarações:", selected_declaracoes)

    def update_progress(self, progress: int):
        print(f"Progress: {progress}%")

    def update_log(self, message):
        print(f"Log: {message}")

    def add_empresa(self):
        """Open window to add a new empresa"""
        EmpresaEditWindow(self.master, None, self.refresh_empresas_list)

    def edit_empresa(self, empresa, index):
        """Open window to edit an existing empresa"""
        EmpresaEditWindow(self.master, empresa, self.refresh_empresas_list)

    def remove_empresa(self, empresa):
        """Remove an empresa after confirmation"""
        if messagebox.askyesno(
            "Confirmar Remoção",
            f"Tem certeza que deseja remover a empresa '{empresa.nome}'?",
            parent=self.master,
        ):
            Dados.remove_empresa(empresa)
            self.refresh_empresas_list()

    def refresh_empresas_list(self):
        """Refresh the empresas checkboxes after add/edit"""
        # Clear existing checkboxes
        for widget in self.empresas_scrollable_frame.winfo_children():
            widget.destroy()

        self.empresas_vars.clear()
        self.empresa_checkboxes.clear()

        # Recreate checkboxes
        for idx, empresa in enumerate(Dados.empresas):
            var = IntVar()
            checkbox = ttk.Checkbutton(
                self.empresas_scrollable_frame, text=empresa.nome, variable=var
            )
            checkbox.pack(anchor=tk.W)
            self.empresas_vars.append(var)
            self.empresa_checkboxes.append(checkbox)

            # Add right-click context menu
            menu = tk.Menu(self.empresas_scrollable_frame, tearoff=0)
            menu.add_command(
                label="Abrir AT",
                command=lambda e=empresa: browser.Browser.abre_portal_das_financas(e),
            )
            menu.add_command(
                label="Abrir e-Fatura",
                command=lambda e=empresa: browser.Browser.abre_efatura(e),
            )
            menu.add_command(
                label="Abrir Segurança Social",
                command=lambda e=empresa: browser.Browser.abre_seguranca_social(e),
            )
            menu.add_command(
                label="Abrir Fundos de Compensação",
                command=lambda e=empresa: browser.Browser.abre_fundos_de_compensacao(e),
            )
            menu.add_separator()
            menu.add_command(
                label="Editar",
                command=lambda e=empresa, i=idx: self.edit_empresa(e, i),
            )
            menu.add_command(
                label="Remover",
                command=lambda e=empresa: self.remove_empresa(e),
            )

            def show_menu(event, m=menu, cb=checkbox):
                m.tk_popup(event.x_root, event.y_root)

            checkbox.bind("<Button-3>", show_menu)


class EmpresaEditWindow:
    """Window for adding or editing an empresa"""

    def __init__(self, parent, empresa=None, on_save_callback=None):
        self.empresa = empresa
        self.on_save_callback = on_save_callback
        self.is_new = empresa is None

        self.window = tk.Toplevel(parent)
        self.window.title("Adicionar Empresa" if self.is_new else "Editar Empresa")
        self.window.geometry("500x600")
        self.window.resizable(False, False)

        # Make modal
        self.window.transient(parent)
        self.window.grab_set()

        # Create form
        form_frame = ttk.Frame(self.window, padding=20)
        form_frame.pack(fill=tk.BOTH, expand=True)

        row = 0

        # Nome
        ttk.Label(form_frame, text="Nome:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.nome_entry = ttk.Entry(form_frame, width=40)
        self.nome_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # Código
        ttk.Label(form_frame, text="Código:").grid(
            row=row, column=0, sticky=tk.W, pady=5
        )
        self.codigo_entry = ttk.Entry(form_frame, width=40)
        self.codigo_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # NIF
        ttk.Label(form_frame, text="NIF:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.nif_entry = ttk.Entry(form_frame, width=40)
        self.nif_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # Password AT
        ttk.Label(form_frame, text="Password AT:").grid(
            row=row, column=0, sticky=tk.W, pady=5
        )
        self.password_at_entry = ttk.Entry(form_frame, width=40, show="*")
        self.password_at_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # NISS
        ttk.Label(form_frame, text="NISS:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.niss_entry = ttk.Entry(form_frame, width=40)
        self.niss_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # Password SS
        ttk.Label(form_frame, text="Password SS:").grid(
            row=row, column=0, sticky=tk.W, pady=5
        )
        self.password_ss_entry = ttk.Entry(form_frame, width=40, show="*")
        self.password_ss_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # Nome do Responsável
        ttk.Label(form_frame, text="Nome do Responsável:").grid(
            row=row, column=0, sticky=tk.W, pady=5
        )
        self.nome_responsavel_entry = ttk.Entry(form_frame, width=40)
        self.nome_responsavel_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # Telefone do Responsável
        ttk.Label(form_frame, text="Telefone do Responsável:").grid(
            row=row, column=0, sticky=tk.W, pady=5
        )
        self.telefone_responsavel_entry = ttk.Entry(form_frame, width=40)
        self.telefone_responsavel_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # Email do Responsável
        ttk.Label(form_frame, text="Email do Responsável:").grid(
            row=row, column=0, sticky=tk.W, pady=5
        )
        self.email_responsavel_entry = ttk.Entry(form_frame, width=40)
        self.email_responsavel_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        # Código Certidão Permanente
        ttk.Label(form_frame, text="Código Certidão Permanente:").grid(
            row=row, column=0, sticky=tk.W, pady=5
        )
        self.codigo_certidao_entry = ttk.Entry(form_frame, width=40)
        self.codigo_certidao_entry.grid(row=row, column=1, pady=5, sticky=tk.EW)
        row += 1

        form_frame.columnconfigure(1, weight=1)

        # Buttons
        button_frame = ttk.Frame(self.window, padding=10)
        button_frame.pack(fill=tk.X)

        ttk.Button(button_frame, text="Guardar", command=self.save).pack(
            side=tk.RIGHT, padx=5
        )
        ttk.Button(button_frame, text="Cancelar", command=self.window.destroy).pack(
            side=tk.RIGHT
        )

        # Populate fields if editing
        if not self.is_new:
            self.populate_fields()

    def populate_fields(self):
        """Populate form fields with empresa data"""
        self.nome_entry.insert(0, self.empresa.nome or "")
        self.codigo_entry.insert(0, self.empresa.codigo or "")
        self.nif_entry.insert(0, self.empresa.nif or "")
        self.password_at_entry.insert(0, self.empresa.password_at or "")
        self.niss_entry.insert(0, self.empresa.niss or "")
        self.password_ss_entry.insert(0, self.empresa.password_ss or "")
        self.nome_responsavel_entry.insert(0, self.empresa.nome_do_responsavel or "")
        self.telefone_responsavel_entry.insert(
            0, self.empresa.telefone_do_responsavel or ""
        )
        self.email_responsavel_entry.insert(0, self.empresa.email_do_responsavel or "")
        self.codigo_certidao_entry.insert(
            0, self.empresa.codigo_certidao_permanente or ""
        )

    def save(self):
        """Save empresa data"""
        # Validation
        nome = self.nome_entry.get().strip()
        nif = self.nif_entry.get().strip()

        if not nome:
            tk.messagebox.showerror("Erro", "Nome é obrigatório", parent=self.window)
            return

        if not nif:
            tk.messagebox.showerror("Erro", "NIF é obrigatório", parent=self.window)
            return

        if self.is_new:
            # Create new empresa
            codigo = self.codigo_entry.get().strip()
            empresa = Empresa(nome, codigo, nif)
            Dados.empresas.append(empresa)
        else:
            # Update existing empresa
            empresa = self.empresa
            empresa.nome = nome
            empresa.codigo = self.codigo_entry.get().strip()
            empresa.nif = nif

        # Update all fields
        empresa.password_at = self.password_at_entry.get().strip() or None
        empresa.niss = self.niss_entry.get().strip() or None
        empresa.password_ss = self.password_ss_entry.get().strip() or None
        empresa.nome_do_responsavel = self.nome_responsavel_entry.get().strip() or None
        empresa.telefone_do_responsavel = (
            self.telefone_responsavel_entry.get().strip() or None
        )
        empresa.email_do_responsavel = (
            self.email_responsavel_entry.get().strip() or None
        )
        empresa.codigo_certidao_permanente = (
            self.codigo_certidao_entry.get().strip() or None
        )

        # Save to file
        Dados.save_empresas()

        # Refresh parent list
        if self.on_save_callback:
            self.on_save_callback()

        self.window.destroy()


def main():
    Dados.load()

    root = Tk()
    main_window = MainWindow(root)

    # Save settings when closing the window
    def on_closing():
        main_window.save_settings()
        root.destroy()

    root.protocol("WM_DELETE_WINDOW", on_closing)
    root.geometry("900x600")
    root.minsize(800, 500)
    root.mainloop()


if __name__ == "__main__":
    main()
