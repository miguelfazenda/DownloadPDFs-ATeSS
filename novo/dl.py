import os
import time
from datetime import datetime
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from autenticacao import Autenticacao
from dados import Empresa
from scraper_recibos_verdes import TipoReciboVerdePrestOUAdquir
from downloader_excel_wintouch import DownloaderWintouch


class EstruturaNomesFicheiros:
    """Placeholder for file naming structure"""

    AT_LISTA_RECIBOS_VERDES_WINTOUCH_PRESTADOS = "RV_Prestados_{}.txt"
    AT_LISTA_RECIBOS_VERDES_WINTOUCH_ADQUIRIDOS = "RV_Adquiridos_{}.txt"
    AT_LISTA_RECIBOS_VERDES_PARA_EXCEL_PRESTADOS = "RV_Excel_{}.xlsx"


class Downloader:
    @staticmethod
    def get_folder_tipo_declaracao(tipo_declaracao, mes):
        """Generate folder name for declaracao type"""
        return f"{tipo_declaracao}_{mes:02d}" if mes != -1 else tipo_declaracao

    @staticmethod
    def gen_novo_nome_ficheiro(template):
        """Generate new filename based on template"""
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        return template.format(timestamp)

    @staticmethod
    def executar(
        empresas: list[Empresa],
        declaracoes: list[Declaracao],
        ano,
        mes,
        download_folder,
        report_progress=None,
        headless=False,
    ):
        # Convert ano and mes to integers (they might be strings from GUI Entry widgets)
        try:
            ano = int(ano) if ano else datetime.now().year
        except (ValueError, TypeError):
            ano = datetime.now().year
        try:
            mes = int(mes) if mes else datetime.now().month
        except (ValueError, TypeError):
            mes = datetime.now().month

        total_empresas = len(empresas)
        estrutura_nomes_ficheiros = EstruturaNomesFicheiros()

        for i_empresa, empresa in enumerate(empresas):
            try:
                download_folder_empresa = os.path.join(download_folder, str(ano))
                os.makedirs(download_folder_empresa, exist_ok=True)

                driver = Downloader.criar_driver(download_folder_empresa, headless)
                if not driver:
                    print("Erro ao criar driver.")
                    return

                try:
                    Autenticacao.autenticar_at(driver, empresa)
                except Exception as ex:
                    print(f"Erro ao autenticar: {ex}")
                    driver.quit()
                    break

                total_declaracoes = len(declaracoes)
                for i_declaracao, declaracao in enumerate(declaracoes):
                    try:
                        if (
                            hasattr(declaracao, "download_function_anual")
                            and declaracao.download_function_anual
                        ):
                            declaracao.download_function_anual(
                                driver=driver,
                                ano=ano,
                                empresa=empresa,
                                download_folder=download_folder_empresa,
                                folder_tipo_declaracao=Downloader.get_folder_tipo_declaracao,
                                gen_novo_nome_ficheiro=Downloader.gen_novo_nome_ficheiro,
                                estrutura_nomes_ficheiros=estrutura_nomes_ficheiros,
                            )
                        elif (
                            hasattr(declaracao, "download_function_mensal")
                            and declaracao.download_function_mensal
                        ):
                            # Determine tipo based on declaracao metadata
                            declaracao.download_function_mensal(
                                driver=driver,
                                ano=ano,
                                mes=mes,
                                empresa=empresa,
                                download_folder=download_folder_empresa,
                                folder_tipo_declaracao=Downloader.get_folder_tipo_declaracao,
                                gen_novo_nome_ficheiro=Downloader.gen_novo_nome_ficheiro,
                                estrutura_nomes_ficheiros=estrutura_nomes_ficheiros,
                            )
                    except Exception as ex:
                        print(f"Erro na declaração {declaracao.nome}: {ex}")
                        import traceback

                        traceback.print_exc()
                    if report_progress:
                        progresso = int(
                            (
                                (
                                    i_empresa / total_empresas
                                    + (
                                        (i_declaracao / total_declaracoes)
                                        / total_empresas
                                    )
                                )
                                * 100
                            )
                        )
                        report_progress(progresso)
                time.sleep(2)
                driver.quit()
            except Exception as ex:
                print(f"Erro geral: {ex}")
                import traceback

                traceback.print_exc()
                break

    @staticmethod
    def criar_driver(download_folder, headless):
        chrome_options = Options()
        prefs = {
            "download.default_directory": os.path.abspath(download_folder),
            "plugins.always_open_pdf_externally": True,
            "profile.default_content_settings.popups": 0,
            "directory_upgrade": True,
            "profile.default_content_setting_values.automatic_downloads": 1,
            "safebrowsing.disable_download_protection": 1,
            "credentials_enable_service": False,
            "profile.password_manager_enabled": False,
            "profile.default_content_setting_values.images": 2,
        }
        chrome_options.add_experimental_option("prefs", prefs)
        chrome_options.add_argument("--disable-search-engine-choice-screen")
        if headless:
            chrome_options.add_argument("--headless")
        driver = webdriver.Chrome(options=chrome_options)
        return driver

    @staticmethod
    def click_button_wait_for_it_to_appear(driver, by):
        found_btn = False
        tries = 0
        while not found_btn and tries < 5:
            btn = driver.find_elements(*by)
            if not btn:
                time.sleep(0.5)
            else:
                found_btn = True
            tries += 1
        if found_btn:
            btn[0].click()
            return True
        return False

    @staticmethod
    def log(tipo, msg):
        print(f"[{tipo}] {msg}")


# --- Declaracao class and static list ---
class Declaracao:
    declaracoes = []

    def __init__(
        self,
        nome,
        tipo,
        download_function_anual=None,
        download_function_mensal=None,
        autenticacao_necessaria: str = None,
    ):
        self.nome = nome
        self.tipo = tipo
        self.download_function_anual = download_function_anual
        self.download_function_mensal = download_function_mensal
        self.autenticacao_necessaria = autenticacao_necessaria
        Declaracao.declaracoes.append(self)

    def __str__(self):
        return self.nome
