import json
import os
from openpyxl import Workbook
from openpyxl.styles import NamedStyle
from datetime import datetime
from tabulate import tabulate
from definicoes import get_export_defs
from scraper_recibos_verdes import (
    TipoReciboVerdePrestOUAdquir,
    ScraperRecibosVerdes,
    ReciboVerde,
)
from downloader_at import DownloaderAT

# Assumes ReciboVerde, RecibosVerdesValores, TipoReciboVerde, TipoReciboVerdePrestOUAdquir are imported from scraper_recibos_verdes.py


class DownloaderExcel:
    @staticmethod
    def download_recibos_verdes_emitidos_excel_anual(
        ano,
        recibos_verdes,
        get_diretorio_empresa,
        declaracao,
        estrutura_nomes_ficheiros,
    ):
        wb = Workbook()
        ws = wb.active
        ws.title = "Recibos verdes"
        headers = [
            "Data",
            "Data de transmissao",
            "Estado",
            "Tipodoc.",
            "Nº",
            "NIF",
            "Nome",
            "País",
            "Recebia a título de ",
            "Descrição",
            "Base",
            "IVA",
            "Selo",
            "IRS",
            "Importancia recebida",
        ]
        ws.append(headers)
        date_style = NamedStyle(name="date_style", number_format="DD/MM/YYYY")
        for recibo in recibos_verdes:
            row = [
                recibo.data_emissao,
                recibo.data_prestacao_servico,
                "Anulado" if recibo.anulado else "Emitido",
                recibo.tipo_doc,
                recibo.num_doc,
                recibo.nif_adquirente,
                recibo.nome_adquirente,
                recibo.pais_adquirente,
                recibo.tipo_recibo_verde if recibo.tipo_recibo_verde else "-",
                recibo.descricao,
                float(recibo.valores.valor_base),
                float(recibo.valores.valor_iva_continente),
                float(recibo.valores.imposto_selo),
                float(recibo.valores.irs_sem_retencao),
                float(recibo.valores.importancia_recebida),
            ]
            ws.append(row)
        for col in ws.columns:
            max_length = 0
            column = col[0].column_letter
            for cell in col:
                try:
                    if cell.value and len(str(cell.value)) > max_length:
                        max_length = len(str(cell.value))
                except:
                    pass
            ws.column_dimensions[column].width = max_length + 2
        file_path = os.path.join(
            get_diretorio_empresa(
                declaracao.AT_LISTA_RECIBOS_VERDES_PARA_EXCEL_PRESTADOS, -1
            ),
            "Lista recibos verdes.xlsx",
        )
        os.makedirs(os.path.dirname(file_path), exist_ok=True)
        wb.save(file_path)


class DownloaderWintouch:
    @staticmethod
    def download_recibos_verdes_emitidos_wintouch(
        driver,
        ano,
        mes,
        tipo,
        empresa_autenticada,
        get_folder_tipo_declaracao,
        download_folder,
        gen_novo_nome_ficheiro,
        estrutura_nomes_ficheiros,
    ):
        # 1. Get recibos verdes list using DownloaderAT
        recibos_verdes = DownloaderAT.get_recibos_verdes_lista(
            driver, ano, mes, tipo, empresa_autenticada
        )
        print(recibos_verdes)

        DownloaderWintouch._export_wintouch(
            recibos_verdes,
            mes,
            tipo,
            get_folder_tipo_declaracao,
            empresa_autenticada,
            download_folder,
            gen_novo_nome_ficheiro,
            estrutura_nomes_ficheiros,
        )

    @staticmethod
    def _export_wintouch(
        recibos_verdes,
        mes,
        tipo,
        get_folder_tipo_declaracao,
        empresa_autenticada,
        download_folder,
        gen_novo_nome_ficheiro,
        estrutura_nomes_ficheiros,
    ):
        import os

        diretorio = os.path.join(
            download_folder,
            get_folder_tipo_declaracao(
                "AT_LISTA_RECIBOS_VERDES_PARA_WINTOUCH_PRESTADOS", mes
            ),
            f"{empresa_autenticada.codigo}-{empresa_autenticada.nif}",
        )
        os.makedirs(diretorio, exist_ok=True)
        if tipo == TipoReciboVerdePrestOUAdquir.Adquirente:
            nome_ficheiro = os.path.join(
                diretorio,
                gen_novo_nome_ficheiro(
                    estrutura_nomes_ficheiros.AT_LISTA_RECIBOS_VERDES_WINTOUCH_ADQUIRIDOS
                ),
            )
        else:
            nome_ficheiro = os.path.join(
                diretorio,
                gen_novo_nome_ficheiro(
                    estrutura_nomes_ficheiros.AT_LISTA_RECIBOS_VERDES_WINTOUCH_PRESTADOS
                ),
            )

        # Guarda o json para debug
        # with open(
        #     os.path.join(diretorio, "recibos_verdes.json"), "w", encoding="utf-8"
        # ) as f:
        #     f.write(
        #         json.dumps(
        #             [recibo.__dict__ for recibo in recibos_verdes],
        #             default=str,
        #             ensure_ascii=False,
        #             indent=2,
        #         )
        #     )

        with open(nome_ficheiro, "w", encoding="utf-8") as file_stream:
            file_stream.write("WCONTAB5.60\n")
            for recibo_verde in recibos_verdes:
                DownloaderWintouch.wintouch_exportar_recibo(file_stream, recibo_verde)

    @staticmethod
    def wintouch_exportar_recibo(file_stream, recibo_verde):
        # --- Export definitions classes and logic ---
        class DefinicoesExportTipoReciboVerde:
            def __init__(
                self,
                contaValBase,
                contaValBaseIsento,
                contaIVA,
                contaSelo,
                contaIRS,
                contaValRecebida,
                diario,
                tipoDoc,
            ):
                self.contaValBase = contaValBase
                self.contaValBaseIsento = contaValBaseIsento
                self.contaIVA = contaIVA
                self.contaSelo = contaSelo
                self.contaIRS = contaIRS
                self.contaValRecebida = contaValRecebida
                self.diario = diario
                self.tipoDoc = tipoDoc

        # Map tipoReciboVerde to the correct sub-definition if needed
        def obter_definicoes_exportacao_recibo(recibo_verde: ReciboVerde):
            return get_export_defs(
                tipo=recibo_verde.tipo,
                tipo_doc=recibo_verde.tipo_doc,
                tipo_recibo_verde=(
                    recibo_verde.tipo_recibo_verde
                    if recibo_verde.tipo_recibo_verde
                    else None
                ),
            )

        def write_linha(
            file_stream,
            recibo_verde,
            definicoes,
            valor,
            codigo_conta,
            natureza,
            num_linha,
        ):
            if valor == 0:
                return False
            codigo_diario = definicoes["diario"]
            codigo_documento = definicoes["tipoDoc"]
            serie = 1
            descricao = (
                (
                    recibo_verde.descricao[:50]
                    if len(recibo_verde.descricao) > 50
                    else recibo_verde.descricao
                )
                if recibo_verde.descricao
                else ""
            )
            dia = recibo_verde.data_emissao.day
            mes = recibo_verde.data_emissao.month
            if recibo_verde.tipo == TipoReciboVerdePrestOUAdquir.Adquirente:
                contribuinte = recibo_verde.nif_prestador_servicos
                nome_entidade = recibo_verde.nome_prestador
            else:
                contribuinte = recibo_verde.nif_adquirente
                nome_entidade = recibo_verde.nome_adquirente
            nome_entidade = (
                (nome_entidade[:50] if len(nome_entidade) > 50 else nome_entidade)
                if nome_entidade
                else ""
            )
            anulado = 1 if recibo_verde.anulado else 0
            valor_str = f"{valor:18.2f}".replace(",", ".")
            linha = f"{'-1':>10}{codigo_diario:>20}{codigo_documento:>20}{serie:>4}{recibo_verde.num_doc:>10}{codigo_conta:<20}{descricao:<50}{valor_str:>18}{natureza:<1}{dia:0>2}{mes:0>2}{contribuinte:<20}F{'':>20}{'C':<1}{num_linha:>5}{'':>20}{nome_entidade:<50}{anulado:<1}\n"
            file_stream.write(linha)
            return True

        definicoes = obter_definicoes_exportacao_recibo(recibo_verde)
        if definicoes is None:
            print(
                f"Sem definições de exportação para recibo verde {recibo_verde.num_doc} do tipo {recibo_verde.tipo_doc} ({recibo_verde.tipo_recibo_verde})"
            )
            return

        # Valor Base
        num_linha = 1
        if recibo_verde.valores.valor_iva_continente > 0:
            if write_linha(
                file_stream,
                recibo_verde,
                definicoes,
                recibo_verde.valores.valor_base,
                definicoes["contaValBase"],
                "C",
                num_linha,
            ):
                num_linha += 1
        else:
            if write_linha(
                file_stream,
                recibo_verde,
                definicoes,
                recibo_verde.valores.valor_base,
                definicoes["contaValBaseIsento"],
                "C",
                num_linha,
            ):
                num_linha += 1
        # Valor IVA
        if write_linha(
            file_stream,
            recibo_verde,
            definicoes,
            recibo_verde.valores.valor_iva_continente,
            definicoes["contaIVA"],
            "C",
            num_linha,
        ):
            num_linha += 1

        # Imposto selo
        if write_linha(
            file_stream,
            recibo_verde,
            definicoes,
            recibo_verde.valores.imposto_selo,
            definicoes["contaSelo"],
            "C",
            num_linha,
        ):
            num_linha += 1

        # IRS sem retenção
        if write_linha(
            file_stream,
            recibo_verde,
            definicoes,
            recibo_verde.valores.irs_sem_retencao,
            definicoes["contaIRS"],
            "D",
            num_linha,
        ):
            num_linha += 1

        # Importancia recebida
        if write_linha(
            file_stream,
            recibo_verde,
            definicoes,
            recibo_verde.valores.importancia_recebida,
            definicoes["contaValRecebida"],
            "D",
            num_linha,
        ):
            num_linha += 1

    @staticmethod
    def gera_tabela_txt_totais(
        totais_tipo_pagamento,
        totais_tipo_adiantamento,
        totais_tipo_adiantamento_pagamento,
        totais_tipo_fatura,
        totais_tipo_recibo,
        totais_anulados,
        mes,
        tipo,
        ano,
        empresa_autenticada,
        download_folder,
        get_folder_tipo_declaracao,
    ):
        total = (
            totais_tipo_pagamento
            + totais_tipo_adiantamento
            + totais_tipo_adiantamento_pagamento
        )
        table = [
            [
                "Pagamento",
                totais_tipo_pagamento.valor_base,
                totais_tipo_pagamento.valor_iva_continente,
                totais_tipo_pagamento.imposto_selo,
                totais_tipo_pagamento.irs_sem_retencao,
                totais_tipo_pagamento.importancia_recebida,
            ],
            [
                "Adiantamento",
                totais_tipo_adiantamento.valor_base,
                totais_tipo_adiantamento.valor_iva_continente,
                totais_tipo_adiantamento.imposto_selo,
                totais_tipo_adiantamento.irs_sem_retencao,
                totais_tipo_adiantamento.importancia_recebida,
            ],
            [
                "Adiant. para despesas",
                totais_tipo_adiantamento_pagamento.valor_base,
                totais_tipo_adiantamento_pagamento.valor_iva_continente,
                totais_tipo_adiantamento_pagamento.imposto_selo,
                totais_tipo_adiantamento_pagamento.irs_sem_retencao,
                totais_tipo_adiantamento_pagamento.importancia_recebida,
            ],
            [
                "Faturas",
                totais_tipo_fatura.valor_base,
                totais_tipo_fatura.valor_iva_continente,
                totais_tipo_fatura.imposto_selo,
                totais_tipo_fatura.irs_sem_retencao,
                totais_tipo_fatura.importancia_recebida,
            ],
            [
                "Total",
                total.valor_base,
                total.valor_iva_continente,
                total.imposto_selo,
                total.irs_sem_retencao,
                total.importancia_recebida,
            ],
            [
                "Total anulados",
                totais_anulados.valor_base,
                totais_anulados.valor_iva_continente,
                totais_anulados.imposto_selo,
                totais_anulados.irs_sem_retencao,
                totais_anulados.importancia_recebida,
            ],
            [
                "Recibos",
                totais_tipo_recibo.valor_base,
                totais_tipo_recibo.valor_iva_continente,
                totais_tipo_recibo.imposto_selo,
                totais_tipo_recibo.irs_sem_retencao,
                totais_tipo_recibo.importancia_recebida,
            ],
        ]
        cabecalho = f"ANO:{ano}\nMES:{mes}\nNIF: {empresa_autenticada.nif}\n\n"
        text = cabecalho + tabulate(
            table,
            headers=["Tipo", "Valor base", "IVA", "Imp. Selo", "IRS", "Recebido"],
            floatfmt=".2f",
        )
        diretorio = os.path.join(
            download_folder,
            get_folder_tipo_declaracao(
                "AT_LISTA_RECIBOS_VERDES_PARA_WINTOUCH_PRESTADOS", mes
            ),
            f"{empresa_autenticada.codigo}-{empresa_autenticada.nif}",
        )
        os.makedirs(diretorio, exist_ok=True)
        if tipo == TipoReciboVerdePrestOUAdquir.Adquirente:
            nome_ficheiro = "Totais adquiridos.txt"
        else:
            nome_ficheiro = "Totais emitidos.txt"
        with open(os.path.join(diretorio, nome_ficheiro), "w", encoding="utf-8") as f:
            f.write(text)
